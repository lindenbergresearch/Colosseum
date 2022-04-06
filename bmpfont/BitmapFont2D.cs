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
using Renoir;
using static Renoir.Util;
using static Renoir.Logger;

#endregion

[Tool]
public class BitmapFont2D : Godot.Node2D {
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
	private string jsonFile;
	private bool lockRefresh;

	/*---------------------------------------------------------------------*/

	public BitmapFont2D() {
		_glyphDimension = Vector2(8, 16);
		_foreground = Color(1, 1, 1);
		_text = "The quick brown fox is dead. !\"§$\"%&/()=?><|#+*'´`/\\";
		_scale = Vector2(1, 1);
		_lineHeight = 2;
		_hasShadow = false;
		_shadow = Color(0, 0, 0);
		_shadowOffset = Vector2(1, 1);

		_offset = 32;

		BitmapFont = new BitmapFont();
	}


	private BitmapFont BitmapFont { get; }


	public string JsonFileName {
		get {
			jsonFile = _imageTexture?.ResourcePath.Substring(6) + ".json";
			return jsonFile;
		}
	}


	/// <summary>
	///     Transfer local properties to BitmapFont
	/// </summary>
	private void ConfigureBitmapFont() {
		BitmapFont.imageTexture = _imageTexture;
		BitmapFont.Offset = _offset;
		BitmapFont.CharsDimension = CharsDimension;
		BitmapFont.GlyphDimension = _glyphDimension;
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
					Vector2(0, yOffset * (GlyphDimension.y + _lineHeight)) +
					ShadowOffset,
					line,
					Shadow
				);

			DrawText(
				Vector2(0, yOffset * (GlyphDimension.y + _lineHeight)),
				line,
				Foreground
			);

			yOffset++;
		}
	}


	/// <summary>
	///     Take properties and try to reload the bitmap-font.
	/// </summary>
	private void Refresh() {
		if (lockRefresh) return;

		try {
			if (_imageTexture?.GetData() == null) {
				trace("Refresh(): no texture data.");
				return;
			}

			if (!BitmapFont.IsLoaded) {
				trace("Refresh(): not loaded!");
				BitmapFont.process();
			}

			if (ValidJsonExists())
				SaveConfigToJSON();

			if (BitmapFont.IsLoaded) {
				CharsDimension = BitmapFont.CharsDimension;
				Update();
			} else {
				error($"{GetType().Name}: Unable to use bitmap-font with that configuration: {Dump(BitmapFont)}");
			}
		} catch (Exception e) {
			trace("Exception: " + e);
		}
	}


	/// <summary>
	/// </summary>
	public void SaveConfigToJSON() {
		if (_imageTexture == null || _imageTexture.GetData() == null || _imageTexture.ResourcePath.Length < 3) return;


		try {
			var _config = new BitmapFontConfig {
				GlyphDimension = {
					X = (int) GlyphDimension.x,
					Y = (int) GlyphDimension.y
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
		}
	}


	public override string ToString() {
		return $"GlyphDimension={GlyphDimension.ToFormatted("D0")} " +
		       $"Scale={Scale} " +
		       $"Offset={Offset} " +
		       $"LineHeight={LineHeight} " +
		       $"HasShadow={HasShadow} " +
		       $"ShadowOffset={ShadowOffset} " +
		       $"CharDimensions={CharsDimension.ToFormatted("D0")}";
	}


	/// <summary>
	/// </summary>
	public override void _Ready() { }


	/*---------------------------------------------------------------------*/

	#region exports

	[Export]
	public bool ForceReload {
		get => false;
		set {
			ConfigureFromJSON(jsonFile);
			ConfigureBitmapFont();
			Refresh();
		}
	}


	[Export]
	public bool SaveToJson {
		get => false;
		set => SaveConfigToJSON();
	}


	/// <summary>
	/// </summary>
	[Export]
	public Texture ImageTexture {
		get => _imageTexture;
		set {
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

			ConfigureBitmapFont();
			Refresh();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Vector2 GlyphDimension {
		get => _glyphDimension;
		set {
			var v = value;

			if ((int) v.x < 1) v.x = 1;
			if ((int) v.y < 1) v.y = 1;

			_glyphDimension = v;

			ConfigureBitmapFont();
			Refresh();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public int Offset {
		get => _offset;
		set {
			_offset = value;
			ConfigureBitmapFont();
			Refresh();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Color Foreground {
		get => _foreground;
		set {
			_foreground = value;
			Refresh();
		}
	}


	/// <summary>
	/// </summary>
	[Export(PropertyHint.MultilineText)]
	public string Text {
		get => _text;
		set {
			_text = value;
			Refresh();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Vector2 TextScale {
		get => _scale;
		set {
			_scale = value;
			Refresh();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Vector2 CharsDimension { get; set; }


	/// <summary>
	/// </summary>
	[Export]
	public float LineHeight {
		get => _lineHeight;
		set {
			_lineHeight = value;
			Refresh();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public Color Shadow {
		get => _shadow;
		set {
			_shadow = value;
			Update();
		}
	}


	/// <summary>
	/// </summary>
	[Export]
	public bool HasShadow {
		get => _hasShadow;
		set {
			_hasShadow = value;
			Update();
		}
	}


	[Export]
	public Vector2 ShadowOffset {
		get => _shadowOffset;
		set {
			_shadowOffset = value;
			Update();
		}
	}

	#endregion
}
