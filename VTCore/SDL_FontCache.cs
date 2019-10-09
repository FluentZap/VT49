using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;


class Glyph
{
  public ushort code;
  public byte width;
  public byte height;
  public byte advance;
  public int textureX;
  public int textureY;
}

namespace VT49
{
  class FC_Font
  {
    public int height, width, heightPadd;
    int maxWidth;
    int baseline;
    int ascent;
    int descent;

    int lineSpacing;
    int letterSpacing;

    int glyph_cache_size;
    int glyph_cache_count;

    SDL_Color default_color;
    // FC_Map* glyphs;
    Dictionary<char, Glyph> Glyphs = new Dictionary<char, Glyph>();

    public IntPtr FontTexture;


    public FC_Font(IntPtr ttf, IntPtr gRenderer, SDL_Color color)
    {
      height = TTF_FontHeight(ttf);
      heightPadd = height + 1;
      ascent = TTF_FontAscent(ttf);
      descent = -TTF_FontDescent(ttf);
      baseline = height - descent;
      default_color = color;

      int w = heightPadd * 12;
      int h = heightPadd * 12;
      // FontTexture = SDL_CreateTexture(gRenderer, SDL_PIXELFORMAT_RGBA8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, w * 16, h * 16);


      IntPtr fontSurface = SDL_CreateRGBSurface(0, h, w, 32, 0xFF000000, 0x00FF0000, 0x0000FF00, 0x000000FF);
      SDL_Rect cursorRect = new SDL_Rect();

      for (int i = 0; i < 144; i++)
      {
        byte[] charByte = new byte[1] { (byte)(i + 33) };
        char letter = Encoding.ASCII.GetString(charByte, 0, 1)[0];
        IntPtr glyphSurface = TTF_RenderUTF8_Blended(ttf, letter.ToString(), color);
        if (glyphSurface != IntPtr.Zero)
        {
          SDL_Surface glyphSurfaceT = Marshal.PtrToStructure<SDL_Surface>(glyphSurface);

          // IntPtr glyphTexture = SDL_CreateTextureFromSurface(gRenderer, glyphSurface);
          // SDL_Rect destRect = new SDL_Rect() { x = (i % 12) * height, y = (i / 12) * height, h = height, w = height };

          SDL_Rect srcRect = new SDL_Rect { x = 0, y = 0, w = glyphSurfaceT.w, h = glyphSurfaceT.h };
          cursorRect.w = srcRect.w;
          cursorRect.h = srcRect.h;

          //drop to next line
          if (cursorRect.x + cursorRect.w > w)
          {
            cursorRect.x = 0;
            cursorRect.y += heightPadd;
          }


          // SDL_Rect destRect = new SDL_Rect() { x = (i % 12) * height, y = (i / 12) * height, h = height, w = height };

          // SDL_SetTextureBlendMode(glyphTexture, SDL_BlendMode.SDL_BLENDMODE_BLEND);

          //Render Glyph
          SDL_SetSurfaceBlendMode(glyphSurface, SDL_BlendMode.SDL_BLENDMODE_NONE);
          // SDL_BlitSurface(glyphSurface, ref srcRect, fontSurface, ref destRect);
          SDL_BlitSurface(glyphSurface, ref srcRect, fontSurface, ref cursorRect);


          if (!Glyphs.ContainsKey(letter))
          {
            Glyphs.Add(letter, new Glyph()
            {
              code = (ushort)(i + 33),
              width = (byte)cursorRect.w,
              height = (byte)cursorRect.h,
              textureX = cursorRect.x,
              textureY = cursorRect.y,
            });
          }

          cursorRect.x += cursorRect.w;
          // SDL_SetTextureBlendMode(glyphTexture, SDL_BlendMode.SDL_BLENDMODE_BLEND);
          // SDL_RenderCopy(gRenderer, glyphTexture, IntPtr.Zero, ref destRect);

          // SDL_DestroyTexture(glyphTexture);
          SDL_FreeSurface(glyphSurface);
        }
      }

      FontTexture = SDL_CreateTexture(gRenderer, SDL_PIXELFORMAT_RGBA8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, w, h);
      SDL_SetTextureBlendMode(FontTexture, SDL_BlendMode.SDL_BLENDMODE_BLEND);
      SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "0");

      SDL_SetRenderTarget(gRenderer, FontTexture);
      SDL_SetRenderDrawColor(gRenderer, 0, 0, 0, 0);
      SDL_RenderClear(gRenderer);

      IntPtr glyphTexture = SDL_CreateTextureFromSurface(gRenderer, fontSurface);

      SDL_RenderCopy(gRenderer, glyphTexture, IntPtr.Zero, IntPtr.Zero);

      SDL_DestroyTexture(glyphTexture);
      SDL_FreeSurface(fontSurface);

      SDL_SetRenderTarget(gRenderer, IntPtr.Zero);
    }

    public void FC_DrawText(IntPtr gRenderer, int x, int y, string text)
    {
      SDL_Rect cursorRect = new SDL_Rect();
      SDL_Rect srcRect = new SDL_Rect();

      cursorRect.x = x;
      cursorRect.y = y;
      for (int i = 0; i < text.Length; i++)
      {
        if (text[i] == ' ' || !Glyphs.ContainsKey(text[i]))
        {
          cursorRect.x += height;
        }
        else
        {
          Glyph g = Glyphs[text[i]];
          srcRect.x = g.textureX;
          srcRect.y = g.textureY;
          srcRect.w = g.width;
          srcRect.h = g.height;

          cursorRect.h = g.height;
          cursorRect.w = g.width;
          SDL_RenderCopy(gRenderer, FontTexture, ref srcRect, ref cursorRect);
          cursorRect.x += g.width;
        }
      }
    }

  }

}
