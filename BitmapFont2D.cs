using System;
using Godot;
using static Util;

/// <summary>
///     Tool node to edit and preview a bitmap font in realtime.
/// </summary>
[Tool]
public class BitmapFont2D : Godot.Node2D {
	private Color _foreground;
	private Color _shadow;
	private Vector2 _glyphDimension;
	private Texture _imageTexture;
	private float _lineHeight;
	private int _offset;
	private Vector2 _scale;
	private string _text;
	private bool _hasShadow;
	private Vector2 _shadowOffset;

	private bool lockRefresh;


	/// <summary>
	///     Setup standard values
	/// </summary>
	public BitmapFont2D() {
		_glyphDimension = Vec(8, 16);
		_foreground = Color(1, 1, 1);
		_text = "The quick brown fox is dead. !\"§$\"%&/()=?><|#+*'´`/\\";
		_scale = Vec(1, 1);
		_lineHeight = 2;
		_hasShadow = false;
		_shadow = Color(0, 0, 0);
		_shadowOffset = Vec(1, 1);

		_offset = 32;

		BitmapFont = new BitmapFont();
	}


	private BitmapFont BitmapFont { get; }


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

			if (_imageTexture != null && _imageTexture.ResourcePath.Length > 3) {
				// test of there is a file equal to the image file ending with json
				var jsonFile = _imageTexture.ResourcePath.Substring(6) + ".json";
				if (System.IO.File.Exists(jsonFile)) {
					GD.Print($"Found a corresponding json config file for this bitmap-font: {jsonFile}. Try to load it...");
					ConfigureFromJSON(jsonFile);
				}
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
	/// 
	/// </summary>
	[Export]
	public Vector2 CharsDimension { get; set; }

	/// <summary>
	/// 
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
	/// 
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
	/// 
	/// </summary>
	[Export]
	public bool HasShadow {
		get => _hasShadow;
		set {
			_hasShadow = value;
			Update();
		}
	}

	[Export()]
	public Vector2 ShadowOffset {
		get => _shadowOffset;
		set {
			_shadowOffset = value;
			Update();
		}
	}


	/// <summary>
	/// Transfer local properties to BitmapFont
	/// </summary>
	private void ConfigureBitmapFont() {
		BitmapFont.imageTexture = _imageTexture;
		BitmapFont.Offset = _offset;
		BitmapFont.CharsDimension = CharsDimension;
		BitmapFont.GlyphDimension = _glyphDimension;
	}


	/// <summary>
	/// Simulates a put pixel taking scale into account.
	/// </summary>
	/// <param name="pos">The position (in normal coordinates)</param>
	/// <param name="color">The color</param>
	private void PutPixel(Vector2 pos, Color color) {
		DrawRect(new Rect2(pos * _scale, _scale), color);
	}


	/// <summary>
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="text"></param>
	private void DrawText(Vector2 pos, string text, Color color) {
		for (var c = 0; c < text.Length; c++) {
			var index = Mathf.Clamp(text[c] - _offset, 0, BitmapFont.GetGlyphCount() - 1);
			var glyph = BitmapFont.GetGlyph(index);


			for (var y = 0; y < GlyphDimension.y; y++)
			for (var x = 0; x < GlyphDimension.x; x++)
				if (glyph.PixelAt(x, y))
					PutPixel(Vec(x + c * GlyphDimension.x, y) + pos, color);
		}
	}


	/// <summary>
	/// Draw bitmap-font to canvas.
	/// </summary>
	public override void _Draw() {
		if (_imageTexture == null || _imageTexture.GetData() == null || BitmapFont == null || BitmapFont.GetGlyphCount() == 0 || !BitmapFont.IsLoaded)
			return;


		var lines = Text.Split('\n');
		var yoffset = 0;

		foreach (var line in lines) {
			if (HasShadow) {
				DrawText(Vec(0, yoffset * (GlyphDimension.y + _lineHeight)) + ShadowOffset, line, Shadow);
			}

			DrawText(Vec(0, yoffset * (GlyphDimension.y + _lineHeight)), line, Foreground);
			yoffset++;
		}
	}


	/// <summary>
	/// Take properties and try to reload the bitmap-font.
	/// </summary>
	private void Refresh() {
		Logger.trace($"Refresh(): {this}");

		if (lockRefresh) return;

		try {
			if (_imageTexture == null || _imageTexture.GetData() == null) return;

			BitmapFont.process();
			//SaveConfigToJSON();


			if (BitmapFont.IsLoaded) {
				CharsDimension = BitmapFont.CharsDimension;
				Update();
			}

			else GD.PrintErr($"{GetType().Name}: Unable to use bitmap-font with that configuration: {Dump(BitmapFont)}");
		}
		catch (Exception e) {
			GD.PrintErr(e);
			Logger.trace("Exception: " + e);
		}
		
		Logger.trace($"After Refresh(): {this}");

	}


	/// <summary>
	/// 
	/// </summary>
	public void SaveConfigToJSON() {
		if (_imageTexture == null || _imageTexture.GetData() == null || _imageTexture.ResourcePath.Length < 3) return;
		var jsonFile = _imageTexture.ResourcePath.Substring(6) + ".json";


		try {
			var _config = new BitmapFontConfig();

			_config.GlyphDimension.X = (int) GlyphDimension.x;
			_config.GlyphDimension.Y = (int) GlyphDimension.y;
			_config.Scale = Scale.x;
			_config.Offset = Offset;
			_config.HasShadow = HasShadow;
			_config.ShadowOffset.X = (int) ShadowOffset.x;
			_config.ShadowOffset.Y = (int) ShadowOffset.y;

			SerializeObject(_config, jsonFile);
		}
		catch (Exception e) {
			GD.PrintErr($"Unable to save bitmap-file config data to json file: {jsonFile}");
		}
	}


	/// <summary>
	/// Setup bitmap properties from json data.
	/// </summary>
	/// <param name="filename"></param>
	public void ConfigureFromJSON(string filename) {
		lockRefresh = true;

		try {
			var _config = DeserializeObject<BitmapFontConfig>(filename);

			GlyphDimension = new Vector2(_config.GlyphDimension.X, _config.GlyphDimension.Y);
			Scale = new Vector2(_config.Scale, _config.Scale);
			Offset = _config.Offset;
			LineHeight = _config.LineHeight;
			HasShadow = _config.HasShadow;
			ShadowOffset = new Vector2(_config.ShadowOffset.X, _config.ShadowOffset.Y);

			GD.Print("Bitmap-font successfully configured by json file.");
		}
		catch (Exception e) {
			GD.PrintErr($"Unable to load bitmap-font config data from json file: {filename}");
		}
		finally {
			lockRefresh = false;
		}
	}


	public override string ToString() {
		return $"GlyphDimension={GlyphDimension} Scale={Scale} Offset={Offset} LineHeight={LineHeight} HasShadow={HasShadow} ShadowOffset={ShadowOffset} CharDimensions={CharsDimension}";
	}


	/// <summary>
	/// </summary>
	public override void _Ready() {
	}
}
