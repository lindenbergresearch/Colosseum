#region header

// 
//    _____
//   (, /   )            ,
//     /__ /  _ __   ___   __
//  ) /   \__(/_/ (_(_)_(_/ (_  CORE LIBRARY
// (_/ ______________________________________/
// 
// 
// Renoir Core Library for the Godot Game-Engine.
// Copyright 2020-2022 by Lindenberg Research.
// 
// www.lindenberg-research.com
// www.godotengine.org
// 

#endregion

#region

using System;
using Godot;
using static Renoir.Util;
using static Renoir.Logger;

#endregion

[Tool]
public class BitmapFont2D : Godot.Node2D {
	private Vector2 _charsDimension;
	private Color _foreground;
	private Vector2 _glyphDimension;
	private bool _hasShadow;
	private Texture _imageTexture;
	private float _lineHeight;
	private int _offset;
	private Vector2 _scale;
	private Color _shadow;
	private Vector2 _shadowOffset;
	private string _text;
	private bool initialized = false;
	private string jsonFile;
	private bool lockRefresh;


	public BitmapFont2D() {
		trace($"{this.GetInstanceId()}/{this.GetHashCode()} +++++ INIT BitmapFont2D() ++++++++++++++");
		Clear();

		BitmapFont = new BitmapFont();
	}


	private BitmapFont BitmapFont { get; }


	public string JsonFileName {
		get {
			jsonFile = _imageTexture?.ResourcePath.Substring(6) + ".json";
			return jsonFile;
		}
	}


	/*---------------------------------------------------------------------*/

	public void Clear() {
		_glyphDimension = Vector2(8, 16);
		_charsDimension = Vector2(32, 8);
		_foreground = Color(1, 1, 1);
		//	_text = "The quick brown fox is dead. !\"§$\"%&/()=?><|#+*'´`/\\";

		_text = "THIS IS UPPERCASE and lowercase\n";
		_text += "!\"§$%&/()=?><|#+*'´`/\\----------\n";
		_text += "1+2*3/4+(5%6)*[7,8] -----------";

		_scale = Vector2(1, 1);
		_lineHeight = 2;
		_hasShadow = true;
		_shadow = Color(0, 0, 0);
		_shadowOffset = Vector2(1, 1);

		_offset = 0;
	}


	/// <summary>
	///     Transfer local properties to BitmapFont
	/// </summary>
	private void ConfigureBitmapFont() {
		trace("Update font config! ***********");
		BitmapFont.imageTexture = _imageTexture;
		BitmapFont.Offset = _offset;
		BitmapFont.CharsDimension = _charsDimension;
		BitmapFont.GlyphDimension = _glyphDimension;

		BitmapFont.IsLoaded = false;
	}


	/// <summary>
	///     Simulates a put pixel taking scale into account.
	/// </summary>
	/// <param name="pos">The position (in normal coordinates)</param>
	/// <param name="color">The color</param>
	private void PutPixel(Vector2 pos, Color color) {
		DrawRect(new Rect2(pos * _scale, _scale), color);
	}


	private void DrawText(Vector2 pos, string text, Color color) {
		for (var c = 0; c < text.Length; c++) {
			var index = Mathf.Clamp(text[c] - _offset, 0, BitmapFont.GetGlyphCount() - 1);
			var glyph = BitmapFont.GetGlyph(index);


			for (var y = 0; y < GlyphDimension.y; y++)
				for (var x = 0; x < GlyphDimension.x; x++)
					if (glyph.PixelAt(x, y))
						PutPixel(Vector2(x + c * GlyphDimension.x, y) + pos, color);
		}
	}


	/// <summary>
	///     Draw bitmap-font to canvas.
	/// </summary>
	public override void _Draw() {
		if (
			_imageTexture?.GetData() == null ||
			BitmapFont == null ||
			BitmapFont.GetGlyphCount() == 0 ||
			!BitmapFont.IsLoaded
		) return;


		var lines = Text.Split('\n');
		var yOffset = 0;

		foreach (var line in lines) {
			if (HasShadow)
				DrawText(
					Vector2(0, yOffset * (_glyphDimension.y + _lineHeight)) +
					_shadowOffset,
					line,
					_shadow
				);

			DrawText(
				Vector2(0, yOffset * (_glyphDimension.y + _lineHeight)),
				line,
				_foreground
			);

			yOffset++;
		}
	}


	/// <summary>
	///     Take properties and try to reload the bitmap-font.
	/// </summary>
	private void Refresh() {
		if (lockRefresh) {
			trace("Refresh Locked.");
			return;
		}

		trace("Refresh Start -------------------------------");

		try {
			if (_imageTexture?.GetData() == null) {
				trace("No texture data.");
				return;
			}

			if (!BitmapFont.IsLoaded) {
				trace("Font has not loaded flag!");
				BitmapFont.process();
				trace($"Process => {BitmapFont}");

				if (BitmapFont.IsLoaded && !ValidJsonExists())
					SaveConfigToJSON();
			}

			if (BitmapFont.IsLoaded)
				trace("Font loaded.");
			else
				error($"Unable to use bitmap-font with that configuration: {this}");
		} catch (Exception e) {
			trace("Exception: " + e);
		}

		trace($"Data: {BitmapFont}");
		trace("Refresh Stop  -------------------------------");
	}


	/// <summary>
	/// </summary>
	public void SaveConfigToJSON() {
		if (_imageTexture == null || _imageTexture.GetData() == null || _imageTexture.ResourcePath.Length < 3) {
			trace("Unable to save to Json. Insufficient data.");
			return;
		}


		try {
			var _config = new BitmapFontConfig {
				GlyphDimension = {
					X = (int) GlyphDimension.x,
					Y = (int) GlyphDimension.y
				},
				CharsDimension = {
					X = (int) CharsDimension.x,
					Y = (int) CharsDimension.y
				},
				Scale = Scale.x,
				Offset = Offset,
				HasShadow = HasShadow,
				ShadowOffset = {
					X = (int) ShadowOffset.x,
					Y = (int) ShadowOffset.y
				}
			};

			SerializeObject(_config, JsonFileName);
		} catch (Exception e) {
			error($"Unable to save bitmap-file config data to json file: '{JsonFileName}'");
			error($"Exception is: {e.Message}");
		}
	}


	public bool ValidJsonExists() {
		try {
			var _config = DeserializeObject<BitmapFontConfig>(JsonFileName);
		} catch (Exception e) {
			warn($"No valid Json file found: '{JsonFileName}'.");
			return false;
		}

		return true;
	}


	/// <summary>
	///     Setup bitmap properties from json data.
	/// </summary>
	/// <param name="filename"></param>
	public void ConfigureFromJSON(string filename) {
		if (!ValidJsonExists())
			return;

		lockRefresh = true;

		try {
			var _config = DeserializeObject<BitmapFontConfig>(filename);

			trace($"Bitmap Json config: {_config}");

			GlyphDimension = Vector2(_config.GlyphDimension.X, _config.GlyphDimension.Y);
			CharsDimension = Vector2(_config.CharsDimension.X, _config.CharsDimension.Y);
			Scale = Vector2(_config.Scale, _config.Scale);
			Offset = _config.Offset;
			LineHeight = _config.LineHeight;
			HasShadow = _config.HasShadow;
			ShadowOffset = Vector2(_config.ShadowOffset.X, _config.ShadowOffset.Y);

			info("Bitmap-font successfully configured by json file.");
		} catch (Exception e) {
			error($"Unable to load bitmap-font config data from json file: {filename}");
			error(e.Message);
		} finally {
			lockRefresh = false;
			Update();
			PropertyListChangedNotify();
		}
	}


	public override string ToString() {
		return $"BITMAP-FONT2D ID={this.GetInstanceId()}/{this.GetHashCode()} GlyphDimension={GlyphDimension} " +
		       $"CharDimensions={CharsDimension}" +
		       $"Scale={Scale} " +
		       $"Offset={Offset} " +
		       $"LineHeight={LineHeight} " +
		       $"HasShadow={HasShadow} " +
		       $"ShadowOffset={ShadowOffset}";
	}


	/// <summary>
	/// </summary>
	public override void _Ready() {
		trace($"{this.GetInstanceId()}/{this.GetHashCode()} +++++ DONE BitmapFont2D() ++++++++++++++");


		if (ValidJsonExists())
			ConfigureFromJSON(jsonFile);

		ConfigureBitmapFont();
		Refresh();

		initialized = true;
	}


	/*---------------------------------------------------------------------*/

	#region exports

	[Export]
	public bool Active {
		get => BitmapFont.IsLoaded;
		set {
			trace($"SET ForceReload: {value}");

			if (!value) {
				BitmapFont.IsLoaded = false;
				return;
			}

			ConfigureFromJSON(jsonFile);
			ConfigureBitmapFont();
			Refresh();
		}
	}


	[Export]
	public bool ClearData {
		get => false;
		set {
			trace($"SET ClearData: {value}");
			_imageTexture = null;
			BitmapFont.Reset();
			Clear();
			Update();
			PropertyListChangedNotify();
		}
	}


	[Export]
	public bool SaveToJson {
		get => false;
		set {
			trace($"SET SaveToJson: {value}");
			SaveConfigToJSON();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Texture ImageTexture {
		get => _imageTexture;
		set {
			trace($"SET ImageTexture: {value}");
			_imageTexture = value;

			BitmapFont.Reset();

			if (_imageTexture == null) {
				error("No bitmap-font texture.");
				return;
			}

			if (_imageTexture.ResourcePath.Length > 3 && ValidJsonExists()) {
				info($"Found a corresponding json config file for this bitmap-font: {JsonFileName}. Try to load it...");
				ConfigureFromJSON(JsonFileName);
			}

			debug($"Texture: {_imageTexture?.ResourcePath.Substring(6)}");

			Update();
			PropertyListChangedNotify();

			if (initialized) {
				ConfigureBitmapFont();
				Refresh();
				Update();
			}
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Vector2 GlyphDimension {
		get => _glyphDimension;
		set {
			trace($"SET GlyphDimension: {value}");
			var v = value;

			if ((int) v.x < 1) v.x = 1;
			if ((int) v.y < 1) v.y = 1;

			_glyphDimension = v;
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Vector2 CharsDimension {
		get => _charsDimension;
		set {
			trace($"SET CharsDimension: {value}");

			_charsDimension = value;
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public int Offset {
		get => _offset;
		set {
			trace($"SET Offset: {value}");
			_offset = value;
			if (initialized) {
				ConfigureBitmapFont();
				Refresh();
				Update();
			}
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Color Foreground {
		get => _foreground;
		set {
			trace($"SET Foreground: {value}");
			_foreground = value;
			if (initialized) {
				ConfigureBitmapFont();
				Refresh();
				Update();
			}
		}
	}


	/// <summary>
	/// </summary>
	[Export(PropertyHint.MultilineText)]
	public string Text {
		get => _text;
		set {
			trace($"SET Text: {value.Substr(0, 12)}...");
			_text = value;

			if (initialized && BitmapFont.IsLoaded) {
				Update();
			}
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Vector2 TextScale {
		get => _scale;
		set {
			trace($"SET TextScale: {value}");
			_scale = value;
			if (initialized) {
				ConfigureBitmapFont();
				Update();
			}
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public float LineHeight {
		get => _lineHeight;
		set {
			trace($"SET LineHeight: {value}");
			_lineHeight = value;
			if (initialized) {
				ConfigureBitmapFont();
				Update();
			}
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Color Shadow {
		get => _shadow;
		set {
			trace($"SET Shadow: {value}");
			_shadow = value;
			if (initialized) {
				ConfigureBitmapFont();
				Update();
			}
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public bool HasShadow {
		get => _hasShadow;
		set {
			trace($"SET HasShadow: {value}");
			_hasShadow = value;
			if (initialized) {
				ConfigureBitmapFont();
				Update();
			}
		}
	}


	[Export]
	public Vector2 ShadowOffset {
		get => _shadowOffset;
		set {
			trace($"SET ShadowOffset: {value}");
			_shadowOffset = value;
			if (initialized) {
				ConfigureBitmapFont();
				Update();
			}
		}
	}

	#endregion
}
