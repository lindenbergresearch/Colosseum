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
/// Bitmap font config object.
/// </summary>
internal class BitmapFontConfig : SerializableDataClass {
	public override int VERSION { get; } = 1;

	public Vector<int> GlyphDimension { get; set; } = new(0, 0);
	public Vector<int> CharsDimension { get; set; } = new(0, 0);
	// public float LineHeight { get; set; } = 2.0f;
	// public float Scale { get; set; } = 1.0f;
	// public bool HasShadow { get; set; }
	public int Offset { get; set; }
	// public int Spacing { get; set; }
	// public Vector<float> ShadowOffset { get; set; } = new(2, 2);


	public override string ToString() {
		return
			$"{nameof(VERSION)}: {VERSION}, " +
			$"{nameof(GlyphDimension)}: " +
			$"{GlyphDimension}, " +
			$"{nameof(CharsDimension)}: " +
			$"{CharsDimension}, " +
			// $"{nameof(LineHeight)}: " +
			// $"{LineHeight}px, " +
			// $"{nameof(Scale)}: " +
			// $"{Scale}px, " +
			// $"{nameof(HasShadow)}: " +
			// $"{HasShadow}, " +
			$"{nameof(Offset)}: " +
			$"{Offset}, ";
		// $"{nameof(Scale)}: " +
		// $"{Scale}px, " +
		// $"{nameof(ShadowOffset)}: " +
		// $"{ShadowOffset}";
	}

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

		public override string ToString() {
			return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
		}
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
		trace("+++ CREATE BitmapFont() +++");
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
		GlyphDimension = Vector2(1, 1);
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
	///     Detect transparent color while retrieving the color of a specific point.
	///     The default is (0, 0)
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private Color DetectTransparentColor(int x = 0, int y = 0) {
		//trace($"{this}");

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
		//trace($"{this}");

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


	public bool validate() {
		if (imageTexture == null || imageTexture?.GetData() == null) {
			error("No image texture has been setup.");
			return false;
		}

		if ((int) GlyphDimension.x < MIN_CHAR_DIMENSION || (int) GlyphDimension.y < MIN_CHAR_DIMENSION) {
			error($"Bad glyph dimension: {GlyphDimension}");
			return false;
		}

		var textureDimension = new Vector2(imageTexture.GetData().GetWidth(), imageTexture.GetData().GetHeight());

		if (textureDimension.x < GlyphDimension.x || textureDimension.y < GlyphDimension.y) {
			error($"Bad texture dimensions: {textureDimension}");
			return false;
		}

		if (GlyphDimension * CharsDimension != textureDimension) {
			error(
				$"Dimensions do not match expected value. " +
				$"glyph={GlyphDimension} * chars={CharsDimension} => " +
				$"{GlyphDimension * CharsDimension} != {textureDimension}"
			);

			return false;
		}

		if ((int) textureDimension.x % (int) GlyphDimension.x != 0) {
			error(
				"No possible character x-alignment found: " +
				$"texture-with={textureDimension.x}px " +
				$"char-width:{GlyphDimension.x}px " +
				$"ratio:{textureDimension.x / GlyphDimension.x} !?"
			);

			return false;
		}

		if ((int) textureDimension.y % (int) GlyphDimension.y != 0) {
			error(
				$"No possible character y-alignment found: " +
				$"texture-height={textureDimension.y}px " +
				$"char-height:{GlyphDimension.y}px " +
				$"ratio:{textureDimension.y / GlyphDimension.y} !?"
			);

			return false;
		}

		return true;
	}

	/// <summary>
	/// </summary>
	public void process() {
		debug(
			$"Loading bitmap-font with: {this} " +
			$"from image: {imageTexture?.ResourcePath.Substring(6)} " +
			$"with {imageTexture?.GetData().GetSize()}px."
		);

		IsLoaded = false;

		if (!validate() || !ProcessImage()) {
			error($"Error loading bitmap-font with current config: {this}!");
			return;
		}

		debug($"Successfully created bitmap-font: {this}");
		IsLoaded = true;
	}


	/// <summary>
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return
			$"GlyphDimension={GlyphDimension} " +
			$"CharsDimension={CharsDimension} " +
			$"Offset={Offset} " +
			$"Count={Count} " +
			$"Loaded={IsLoaded}" +
			$"Texture={imageTexture?.ResourcePath.Substring(6)}";
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
