using SkiaSharp;
using Svg.Skia;

namespace Tamarix.Views
{
    public class Image : View
    {
        IImageData? imageData;

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
            var paint = new SKPaint();
            paint.Color = SKColors.White;
            paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White, SKBlendMode.SrcIn);
            if (imageData != null && imageData is SVGImageData)
                (imageData as SVGImageData)!.Draw(canvas, X, Y, Width, Height, paint);
        }

        /// <summary>
        /// Loads SVG image from external file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Image FromSVG(string path, int w, int h)
        {
            var inst = new Image();
            inst.imageData = new SVGImageData(path);
            inst.Width = w;
            inst.Height = h;

            return inst;
        }

        /// <summary>
        /// Loads SVG image from internal file or stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Image FromSVGStream(Stream? stream, int w, int h)
        {
            var inst = new Image();
            inst.imageData = SVGImageData.FromStream(stream);
            inst.Width = w;
            inst.Height = h;

            return inst;
        }

        public static Image FromResource(Type resource_holder, string name, int w, int h)
        {
            var inst = new Image();
            var res = ResourceManager.GetAsset(resource_holder, name);
            if (res == null)
                throw new Exception($"Error loading resource: {resource_holder}:{name}");

            if (name.EndsWith(".svg"))
                inst.imageData = SVGImageData.FromStream(res);
            else
                throw new Exception($"Error unsupported format: {name}");

            inst.Width = w;
            inst.Height = h;

            return inst;
        }
    }
    public interface IImageData
    {
        public void Load(string path);
        public void Draw(SKCanvas canvas, int x, int y, int w, int h, SKPaint? paint = null);
        public void Dispose();
        public int Width { get; }
        public int Height { get; }
        public string Source { get; set; }
    }

    public class SVGImageData : IImageData
    {
        public SVGImageData(string path)
        {
            Load(path);
        }
        public SVGImageData()
        {

        }
        public SKSvg svg = new SKSvg();

        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();


        string IImageData.Source { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Draw(SKCanvas canvas, int x, int y, int w, int h, SKPaint? paint = null)
        {
            // Get drawing surface bounds
            var drawBounds = new SKRect(x, y, x + w, y + h);

            // Get bounding rectangle for SVG image
            var boundingBox = svg.Picture!.CullRect;

            // Translate and scale drawing canvas to fit SVG image
            canvas.Translate(drawBounds.MidX, drawBounds.MidY);
            canvas.Scale(0.9f *
                Math.Min(drawBounds.Width / boundingBox.Width,
                    drawBounds.Height / boundingBox.Height));
            canvas.Translate(-boundingBox.MidX, -boundingBox.MidY);

            // Now finally draw the SVG image
            canvas.DrawPicture(svg.Picture, paint);

            // Optional -> Reset the matrix before performing more draw operations
            canvas.ResetMatrix();
        }
        /// <summary>
        /// Load the data
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                svg.Load(stream);
            }
        }

        /// <summary>
        /// Create SVG image data from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SVGImageData? FromStream(Stream? stream)
        {
            if (stream != null)
            {
                var result = new SVGImageData();
                result.svg.Load(stream!);
                return result;
            }
            return null;
        }
    }
}
