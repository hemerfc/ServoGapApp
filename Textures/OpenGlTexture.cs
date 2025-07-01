using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ServoGapApp.Textures
{
    public sealed class OpenGlTexture : IDisposable
    {
        private readonly int _handle;
        private bool _disposedValue;

        public OpenGlTexture()
        {
            _handle = GL.GenTexture();
        }

        public void LoadFromFile(string path)
        {
            Use();
            var image = Image.Load<Rgba32>(path);

            //ImageSharp counts (0, 0) as top-left, OpenGL wants it to be bottom-left. fix.
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            //Convert ImageSharp's format into a byte array, so we can use it with OpenGL.
            var pixels = new List<byte>(4 * image.Width * image.Height);
            
            image.ProcessPixelRows(pixelAccessor =>
            {
                for (int y = 0; y < pixelAccessor.Height; y++)
                {
                    var row = pixelAccessor.GetRowSpan(y);

                    // Using row.Length helps JIT to eliminate bounds checks when accessing row[x].
                    for (int x = 0; x < row.Length; x++)
                    {                    
                        pixels.Add(row[x].R);
                        pixels.Add(row[x].G);
                        pixels.Add(row[x].B);
                        pixels.Add(row[x].A);
                    }
                }
            });
            
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        ~OpenGlTexture()
        {
            GL.DeleteTexture(_handle);
        }
		
        public void Dispose()
        {
            if (_disposedValue)
            {
                return;
            }

            GL.DeleteTexture(_handle);

            _disposedValue = true;
            
            GC.SuppressFinalize(this);
        }
    }
}