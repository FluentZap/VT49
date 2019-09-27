using System;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace VT49
{
  class FC_Font
  {
    int height, width;
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

    public FC_Font(IntPtr ttf, IntPtr gRenderer, SDL_Color color)
    {

      height = TTF_FontHeight(ttf);
      ascent = TTF_FontAscent(ttf);
      descent = -TTF_FontDescent(ttf);
      baseline = height - descent;
      default_color = color;

      int w = height * 12;
      int h = height * 12;
      
    }
  }

}