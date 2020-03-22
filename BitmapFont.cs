using System;
using Godot;
using static Util;

/// <summary>
/// </summary>
public class BitmapGlyph {
	/// <summary>
	/// </summary>
	private readonly int[,] data;
	private readonly int height;

	private readonly int width;


	/// <summary>
	///     Construct a new bitmap glyph with a given dimension.
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public BitmapGlyph(int width, int height) {
		this.width = width;
		this.height = height;

		data = new int[width, height];
	}


	/// <summary>
	///     Parses the image data and converts it to a 1-bit data array.
	/// </summary>
	/// <param name="image"></param>
	/// <param name="offset"></param>
	/// <param name="transparent"></param>
	public void Parse(Image image, Vector2 offset, Color transparent) {
		image.Lock();

		for (var j = 0; j < height; j++)
		for (var i = 0; i < width; i++) {
			var pixel = image.GetPixel((int) offset.x + i, (int) offset.y + j);

			if (pixel == transparent) data[i, j] = 0;
			else data[i, j] = 1;
		}

		image.Unlock();
	}


	/// <summary>
	///     Tests for pixel at the given coordinate
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public bool PixelAt(int x, int y) {
		return data[x, y] == 1;
	}
}


/// <summary>
///     Simple class to load and use plain bitmap files as bitmap font in Godot.
/// </summary>
public class BitmapFont {
	/// <summary>
	///     Set default values on creation
	/// </summary>
	public BitmapFont() {
		Offset = 32;
		Count = 256;

		CharsDimension = Vec(1, 1);
		GlyphDimension = Vec(8, 16);
		TransparentColor = Color(0, 0, 0, 0);
	}


	/// <summary>
	///     The color which indicates the transparent background.
	/// </summary>
	public Color TransparentColor { get; set; }

	/// <summary>
	///     The the characters count vertical and horizontal.
	///     Will be autodetect.
	/// </summary>
	public Vector2 CharsDimension { get; set; }

	/// <summary>
	///     The dimension of one character in pixels.
	/// </summary>
	public Vector2 GlyphDimension { get; set; }

	/// <summary>
	///     The offset of the character index.
	/// </summary>
	public int Offset { get; set; }

	/// <summary>
	///     The character count.
	/// </summary>
	public int Count { get; set; }


	/// <summary>
	///     The source image
	/// </summary>
	public Texture imageTexture { get; set; }


	public BitmapGlyph[] Glyphs { get; set; }


	/// <summary>
	///     Detects the correct char dimension based on the glyph dimension
	/// </summary>
	public void DetectCharsDimension() {
		if (imageTexture == null) return;

		if ((int) GlyphDimension.x == 0 || (int) GlyphDimension.y == 0) return;

		var idim = new Vector2(imageTexture.GetData().GetWidth(), imageTexture.GetData().GetHeight());

		if ((int) idim.x % (int) GlyphDimension.x != 0)
			throw new BitmapFontException($"The width of the image: {idim.x}px doesn't fit to the glyphs width: {GlyphDimension.x}px!");

		if ((int) idim.y % (int) GlyphDimension.y != 0)
			throw new BitmapFontException($"The height of the image: {idim.y}px doesn't fit to the glyphs height: {GlyphDimension.y}px!");

		CharsDimension = new Vector2(idim.x / GlyphDimension.x, idim.y / GlyphDimension.y);
	}


	/// <summary>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private Color DetectTransparentColor(int x = 0, int y = 0) {
		var image = imageTexture.GetData();

		image.Lock();
		var c = image.GetPixel(x, y);
		image.Unlock();

		return c;
	}


	/// <summary>
	/// </summary>
	private void ProcessImage() {
		var image = imageTexture.GetData();
		var cindex = 0;

		var transparentColor = DetectTransparentColor();


		Glyphs = new BitmapGlyph[(int) CharsDimension.x * (int) CharsDimension.y];

		for (var y = 0; y < CharsDimension.y; y++)
		for (var x = 0; x < CharsDimension.x; x++) {
			var glyph = new BitmapGlyph((int) GlyphDimension.x, (int) GlyphDimension.y);
			glyph.Parse(image, Vec(x * GlyphDimension.x, y * GlyphDimension.y), transparentColor);
			Glyphs[cindex++] = glyph;
		}
	}


	/// <summary>
	/// </summary>
	public void process() {
		if (imageTexture == null || imageTexture.GetData() == null) return;

		GD.Print($"Load bitmap-font from texture: {imageTexture.ResourceName}...");

		DetectCharsDimension();
		ProcessImage();


		GD.Print($"Create bitmapfont: {this}");
	}


	/// <summary>
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return $"{GetType().Name}: GlyphDimention={GlyphDimension} CharsDimension={CharsDimension} Offset={Offset} Count={Count} Texture={imageTexture.ResourceName}";
	}
}

/// <summary>
///     Bitpmap Font Exception
/// </summary>
public class BitmapFontException : Exception {
	public BitmapFontException() {
	}


	public BitmapFontException(string message) : base(message) {
	}


	public BitmapFontException(string message, Exception innerException) : base(message, innerException) {
	}
}
