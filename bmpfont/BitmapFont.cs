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

/*---------------------------------------------------------------------*/

#region font config

/// <summary>
/// </summary>
internal class BitmapFontConfig : SerializableDataClass {
	public override int VERSION { get; } = 1;

	public Vector<int> GlyphDimension { get; set; } = new(0, 0);
	public float LineHeight { get; set; } = 2.0f;
	public float Scale { get; set; } = 1.0f;
	public bool HasShadow { get; set; } = false;
	public int Offset { get; set; } = 0;
	public Vector<float> ShadowOffset { get; set; } = new(2, 2);


	/// <summary>
	///     POCO for holding 2D vector data
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Vector<T> {
		public Vector() { }


		public Vector(T x, T y) {
			X = x;
			Y = y;
		}


		public T X { get; set; }
		public T Y { get; set; }
	}
}

#endregion

/*---------------------------------------------------------------------*/


#region bitmap glyph

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

		try {
			for (var j = 0; j < height; j++)
				for (var i = 0; i < width; i++) {
					var pixel = image.GetPixel((int) offset.x + i, (int) offset.y + j);

					if (pixel == transparent) data[i, j] = 0;
					else data[i, j] = 1;
				}
		} catch (Exception e) {
			trace($"Exception while drawing: {e}");
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

#endregion

/*---------------------------------------------------------------------*/


#region bitmap font

/// <summary>
///     Simple class to load and use plain bitmap files as bitmap font in Godot.
/// </summary>
public class BitmapFont {
	public const int MIN_CHAR_DIMENSION = 4;

	private BitmapGlyph[] _glyphs;


	/// <summary>
	///     Set default values on creation
	/// </summary>
	public BitmapFont() {
		Reset();
	}

	/*---------------------------------------------------------------------*/

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

	/// <summary>
	/// Indicates a loaded font.
	/// </summary>
	public bool IsLoaded { get; set; }

	/// <summary>
	/// Reset font parameter.
	/// </summary>
	public void Reset() {
		Offset = 32;
		Count = 256;

		CharsDimension = Vector2(1, 1);
		GlyphDimension = Vector2(8, 16);
		TransparentColor = Color(0, 0, 0, 0);
		IsLoaded = false;
	}


	/*---------------------------------------------------------------------*/

	/// <summary>
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public BitmapGlyph GetGlyph(int index) {
		if (_glyphs == null || _glyphs.Length <= index) return null;

		return _glyphs[index];
	}


	public int GetGlyphCount() {
		if (_glyphs != null) return _glyphs.Length;
		return -1;
	}


	/// <summary>
	///     Detects the correct char dimension based on the glyph dimension
	/// </summary>
	public void DetectCharsDimension() {
		trace($"{this}");

		if (imageTexture == null) return;

		if ((int) GlyphDimension.x < MIN_CHAR_DIMENSION || (int) GlyphDimension.y < MIN_CHAR_DIMENSION) {
			trace($"Bad glyph dimension: {GlyphDimension}");
			return;
		}

		var idim = new Vector2(imageTexture.GetData().GetWidth(), imageTexture.GetData().GetHeight());

		if (idim.x < MIN_CHAR_DIMENSION || idim.y < MIN_CHAR_DIMENSION) {
			trace($"Bad texture dimensions: {idim}");
			return;
		}

		if ((int) idim.x % (int) GlyphDimension.x != 0) {
			error(
				"No possible character x-alignment found: " +
				$"texture-with={idim.x}px " +
				$"char-width:{GlyphDimension.x}px " +
				$"ratio:{idim.x / GlyphDimension.x} !?"
			);

			return;
		}

		if ((int) idim.y % (int) GlyphDimension.y != 0) {
			error(
				$"No possible character y-alignment found: " +
				$"texture-height={idim.y}px " +
				$"char-height:{GlyphDimension.y}px " +
				$"ratio:{idim.y / GlyphDimension.y} !?"
			);

			return;
		}

		// throw new BitmapFontException($"The height of the image: {idim.y}px doesn't fit to the glyphs height: {GlyphDimension.y}px!");
		// throw new BitmapFontException($"The width of the image: {idim.x}px doesn't fit to the glyphs width: {GlyphDimension.x}px!");

		CharsDimension = new Vector2(idim.x / GlyphDimension.x, idim.y / GlyphDimension.y);
		info($"Character alignment is: {CharsDimension}");
	}


	/// <summary>
	///     Detect transparent color while retrieving the color of a specific point.
	///     The default is (0, 0)
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private Color DetectTransparentColor(int x = 0, int y = 0) {
		trace($"{this}");

		var image = imageTexture.GetData();

		image.Lock();
		var color = image.GetPixel(x, y);
		image.Unlock();

		return color;
	}


	/// <summary>
	///     Try to process the given image with the given configuration.
	/// </summary>
	/// <returns>True if the image could be processed to a bitmap-font</returns>
	private bool ProcessImage() {
		trace($"{this}");

		var image = imageTexture.GetData();
		var cindex = 0;

		if ((int) CharsDimension.x < MIN_CHAR_DIMENSION || (int) CharsDimension.y < MIN_CHAR_DIMENSION) return false;

		if (CharsDimension.x * CharsDimension.y > 512) {
			error($"Suspicious character alignment: {CharsDimension.x} x {CharsDimension.y} !?");
			Reset();
			return false;
		}

		var transparentColor = DetectTransparentColor();

		_glyphs = new BitmapGlyph[(int) CharsDimension.x * (int) CharsDimension.y];

		for (var y = 0; y < CharsDimension.y; y++)
			for (var x = 0; x < CharsDimension.x; x++) {
				var glyph = new BitmapGlyph((int) GlyphDimension.x, (int) GlyphDimension.y);
				glyph.Parse(image, Vector2(x * GlyphDimension.x, y * GlyphDimension.y), transparentColor);

				if (cindex < _glyphs.Length)
					_glyphs[cindex] = glyph;
				else error($"Impossible: {CharsDimension} index: {cindex}");

				cindex++;
			}

		return true;
	}


	/// <summary>
	/// </summary>
	public void process() {
		trace($"{this}");
		IsLoaded = false;
		CharsDimension = Vector2(0, 0);

		if (imageTexture == null || imageTexture?.GetData() == null) {
			trace("No image texture has been setup.");
			return;
		}

		info(
			$"{GetType().Name}: Loading bitmap-font with: {this} " +
			$"from image: {imageTexture.ResourceName} " +
			$"having {imageTexture.GetData().GetSize()}px."
		);

		DetectCharsDimension();

		if (!ProcessImage()) {
			error($"{GetType().Name}: Error loading bitmap-font with current config: {Dump(this)}!");
			return;
		}


		info($"{GetType().Name}: Created bitmap-font: {this}");
		IsLoaded = true;
	}


	/// <summary>
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return
			$"{GetType().Name}: " +
			$"GlyphDimension={GlyphDimension} " +
			$"CharsDimension={CharsDimension} " +
			$"Offset={Offset} " +
			$"Count={Count} " +
			$"Texture={imageTexture?.ResourceName}";
	}
}

#endregion

/*---------------------------------------------------------------------*/


#region bitmap exception

/// <summary>
///     Bitpmap Font Exception
/// </summary>
public class BitmapFontException : Exception {
	public BitmapFontException() { }


	public BitmapFontException(string message) : base(message) { }


	public BitmapFontException(string message, Exception innerException) : base(message, innerException) { }
}

#endregion
