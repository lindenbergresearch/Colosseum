using Godot;
using static Util;

[Tool]
public class BitmapFont2D : Godot.Node2D {
	private int _count;
	private Color _foreground;
	private Vector2 _glyphDimension;
	private Texture _imageTexture;
	private int _offset;
	private Vector2 _scale;
	private string _text;


	/// <summary>
	///     Setup standard values
	/// </summary>
	public BitmapFont2D() {
		_glyphDimension = Vec(8, 16);
		_foreground = Color(1, 1, 1);
		_text = "The quick brown fox is dead. !\"§$\"%&/()=?><|#+*'´`/\\";
		_scale = Vec(1, 1);

		BitmapFont = new BitmapFont();
	}


	private BitmapFont BitmapFont { get; }

	[Export]
	public Texture ImageTexture {
		get => _imageTexture;
		set {
			_imageTexture = value;
			Update();
		}
	}

	[Export]
	public Vector2 GlyphDimension {
		get => _glyphDimension;
		set {
			_glyphDimension = value;
			Update();
		}
	}

	[Export]
	public int Offset {
		get => _offset;
		set {
			_offset = value;
			Update();
		}
	}

	[Export]
	public int Count {
		get => _count;
		set {
			_count = value;
			Update();
		}
	}


	[Export]
	public Color Foreground {
		get => _foreground;
		set {
			_foreground = value;
			Update();
		}
	}

	[Export]
	public string Text {
		get => _text;
		set {
			_text = value;
			Update();
		}
	}

	[Export]
	public Vector2 TextScale {
		get => _scale;
		set {
			_scale = value;
			Update();
		}
	}


	/// <summary>
	///     Simulates a put pixel taking scale into account.
	/// </summary>
	/// <param name="pos">The position (in normal coordinates)</param>
	/// <param name="color">The color</param>
	private void PutPixel(Vector2 pos, Color color) {
		var scaledPos = pos * _scale;
		DrawRect(new Rect2(scaledPos, _scale), color);
	}


	/// <summary>
	///     Draw font
	/// </summary>
	public override void _Draw() {
		PutPixel(new Vector2(),);
	}


	/// <summary>
	/// </summary>
	public override void _Ready() {
	}
}
