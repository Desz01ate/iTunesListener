using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iTunesListener
{

    class ImageHelper

    {

        public static Image GetImage(ref PlayerInstance player)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(),$"{player.Track.Album}.png");
            if (File.Exists(path))
            {
                //byte[] imgdata = System.IO.File.ReadAllBytes(path);
                return Image.FromFile(path);
            }
            else
            {
                var images = HTMLHelper.GetImages($"{player.Track.Name} {player.Track.Album} {player.Track.Artist} album artwork", HTMLHelper.Image_source).Result.First();
                WebClient client = new WebClient();
                var byteArray = client.DownloadData(images);
                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(byteArray));
                x.Save(path);
                using (var ms = new MemoryStream(byteArray))
                { 
                    return Image.FromStream(ms);
                }
            }
            
        }

        public static Color GetMostUsedColor(Bitmap bitMap)
        {
            /*
            var colorThief = new ColorThiefDotNet.ColorThief();
            var result = colorThief.GetColor(bitMap, 1);
            byte R, G, B;
            if (result.Color.R > result.Color.G && result.Color.R > result.Color.B)
            {
                R = (byte)(result.Color.R * 1.5);
                G = result.Color.G;
                B = result.Color.B;
            }
            else if (result.Color.G > result.Color.R && result.Color.G > result.Color.B)
            {
                R = result.Color.R;
                G = (byte)(result.Color.G * 1.5);
                B = result.Color.B;
            }
            else if (result.Color.B > result.Color.R && result.Color.B > result.Color.G)
            {
                R = result.Color.R;
                G = result.Color.G;
                B = (byte)(result.Color.B* 1.5);
            }

            //return Color.FromArgb(result.Color.R, result.Color.G, result.Color.B);
            return Color.FromArgb(int.Parse(result.Color.ToHexAlphaString()));
            */
            var colorIncidence = new Dictionary<int, int>();

            var sampleBlock = new[] {
                new[] { 0,bitMap.Width * 0.1, 0,bitMap.Height * 0.1 }, //left-top
                new[] { bitMap.Width * 0.9,bitMap.Width, bitMap.Width * 0.9, bitMap.Height}, //bitMap.Height * 0.1 ,bitMap.Height}, //right-top
                //new[] { bitMap.Height * 0.9, bitMap.Width, bitMap.Height * 0.9,bitMap.Height }, //left-bottom
                //new[] { bitMap.Width * 0.9, bitMap.Height * 0.9 }  //right-bottom
            };
            /*
            for (var i = 0; i < sampleBlock.Length; i++)
            {
                //var x_start = sampleBlock[i][0];
                //var x_end = sampleBlock[i][1];
                //var y_start = sampleBlock[i][2];
                //var y_end = sampleBlock[i][3];
                for (int x = (int)sampleBlock[i][0]; x < (int)sampleBlock[i][1]; x++)
                {
                    for (int y = (int)sampleBlock[i][2]; y < (int)sampleBlock[i][3]; y++)
                    {
                        var pixelColor = bitMap.GetPixel(x, y).ToArgb();
                        if (colorIncidence.Keys.Contains(pixelColor))
                            colorIncidence[pixelColor]++;
                        else
                            colorIncidence.Add(pixelColor, 1);
                    }
                }
            }
            */

            for (var x = 0; x < bitMap.Size.Width; x++)
                for (var y = 0; y < bitMap.Size.Height; y++)
                {
                    var pixelColor = bitMap.GetPixel(x, y).ToArgb();
                    if (colorIncidence.Keys.Contains(pixelColor))
                        colorIncidence[pixelColor]++;
                    else
                        colorIncidence.Add(pixelColor, 1);
                }

            if (colorIncidence.Count == 1)
                return Color.FromArgb(colorIncidence.First().Key);
            //var r = colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => Color.FromArgb(x.Key), x => x.Value);
            //double avg = colorIncidence.Average(x => x.Key);
            // return Color.FromArgb((int)avg);
            //return Color.FromArgb(colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key);
            var max = colorIncidence.Keys.Max();
            var min = colorIncidence.Keys.Min();
            Int64 dataRange = colorIncidence.Keys.Max() - colorIncidence.Keys.Min();
            if (max < 0 && min < 0)
                dataRange = dataRange * -1;
            Int64[] lowerbound = new Int64[10];
            Int64[] upperbound = new Int64[10];
            lowerbound[0] = colorIncidence.Keys.Min();
            upperbound[0] = lowerbound[0] + dataRange;
            for (var i = 1; i < lowerbound.Length; i++)
            {

                lowerbound[i] = dataRange * (i) + 1;
            }
            for (var i = 1; i < upperbound.Length - 1; i++)
            {
                upperbound[i] = (dataRange * i);
            }
            upperbound[9] = colorIncidence.Keys.Max();
            var frequenceLength = MathHelper.FrequencyDistribution(colorIncidence.Keys.Select(x => (long)x).ToList(), lowerbound, upperbound);
            return Color.FromArgb(colorIncidence.Where(x => frequenceLength[0] < x.Key && x.Key < frequenceLength[1]).OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key);
        }

    }
}
