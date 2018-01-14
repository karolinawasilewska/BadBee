using BadBee.Core.DAL;
using BadBee.Core.Models;
using BadBee.Core.MyResources;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
namespace BadBee.Core.Providers
{
   public class PDFProvider
    {
        public string GetPath()
        {
            string path = string.Format("{0}\\{1}", System.Web.HttpContext.Current.Server.MapPath("~/Images/Pictures"), "BadBeeCatalog.pdf");
            return string.Format("{0}\\{1}", System.Web.HttpContext.Current.Server.MapPath("~/Images/Pictures"), "BadBeeCatalog.pdf");
           
        }
        public string GetYears(string dateFrom, string dateTo)
        {
            if (dateFrom == "0" && dateTo == "0") 
            {
                return string.Empty;
            }
            else if (dateFrom != "0" && dateTo == "0")
            {
                return string.Format(dateFrom + "->");
            }
            else if (dateFrom == "0" && dateTo != "0")
            { 
                return string.Format("->" + dateTo);
            }
            else
            {
                return string.Format(dateFrom + "->" + dateTo);
            }
           
        }
        public void GeneratePDFCatalog()
        {
            List<CvlItem> data = new List<CvlItem>();


            using (BadBeeEntities bbe = new BadBeeEntities())
            {
              data  = (from item in bbe.Item
                             join model in bbe.Model
                                 on item.ModelId equals model.ModelId
                             join serie in bbe.Serie
                                on model.SerieId equals serie.SerieId
                            join brand in bbe.Brand
                                on serie.BrandId equals brand.BrandId
                            join year in bbe.Year
                                on model.YearId equals year.YearId
                            join datefrom in bbe.Date
                                on year.DateFromFK.DateId equals datefrom.DateId
                            join dateto in bbe.Date
                                on year.DateToFK.DateId equals dateto.DateId
                            join badbee in bbe.BadBee
                                on item.BadBeeId equals badbee.BadBeeId
                            join wva in bbe.Wva
                                on badbee.WvaId equals wva.WvaId
                            join system in bbe.Systems
                                on badbee.SystemId equals system.SystemId
                            join dimension in bbe.Dimension
                                on badbee.DimensionId equals dimension.DimensionId
                            join width in bbe.Width
                                on dimension.WidthId equals width.WidthId
                            join height in bbe.Height
                                on dimension.HeightId equals height.HeightId
                            join thickness in bbe.Thickness
                                on dimension.ThicknessId equals thickness.ThicknessId
                                
                             select new CvlItem
                             {
                                 Brand = brand.Name,
                                 Serie = serie.Name,
                                 Model=model.Name,
                                 DateFrom=year.DateFromFK.Date1,
                                 DateTo=year.DateToFK.Date1,
                                 BadBeeNumber = badbee.BadBeeNo,
                                 Fr=badbee.FR,
                                 Wva = wva.WvaNo,
                                 WvaDesc=wva.Description,
                                 Size = height.Height1 + "x" + width.Width1 + "x"+thickness.Thickness1,
                                 BrakeSystem= system.Abbreviation,
                                 Height =height.Height1.ToString(),
                                 Thickness =thickness.Thickness1.ToString(),
                                 Width =width.Width1.ToString(),
                                 
                             }).OrderBy(q=>q.Brand).ThenBy(q=>q.Serie).ThenBy(q=>q.Model).ThenBy(q=>q.Fr).ToList();

            }

          
            PdfDocument document = new PdfDocument();
            int maxWidth = 595;
            int maxHeight = 845;
            int PageNo = 0;
            // Create an empty page or load existing
            PdfPage page = document.AddPage();
            //    // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawImage2(gfx, PDFResources.BadBeeCover, 0, 0, maxWidth, maxHeight);
            gfx.Dispose();

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            gfx.Dispose();

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            gfx.Dispose();

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            DrawImage2(gfx, PDFResources.Page3_applications, 0, 0, maxWidth, maxHeight);
            gfx.Dispose();

            
            PageNo = AddDataPages(gfx, data, page, document, maxHeight, maxWidth);
            
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            DrawImage2(gfx, PDFResources.Page_drawing, 0, 0, maxWidth, maxHeight);
            gfx.Dispose();

            PageNo = AddPicturePages(gfx, page, document, maxHeight, maxWidth, PageNo);

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            gfx.Dispose();

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
      //      DrawImage2(gfx, Properties.Resources.end_page, 0, 0, maxWidth, maxHeight);
            gfx.Dispose();

            // Save and start View
            document.Save(GetPath());
            Process.Start(GetPath());
        }
        XFont font20 = new XFont("Verdana", 20, XFontStyle.Bold);
        XFont font19 = new XFont("Verdana", 19, XFontStyle.Bold);
        XFont font18 = new XFont("Verdana", 18, XFontStyle.Bold);
        XFont font16 = new XFont("Verdana", 16, XFontStyle.Bold);
        XFont font10 = new XFont("Verdana", 10, XFontStyle.Bold);
        XFont font6 = new XFont("Verdana", 6, XFontStyle.Bold);
        XFont font5 = new XFont("Verdana", 5, XFontStyle.Bold);
        XFont font8 = new XFont("Verdana", 8, XFontStyle.Bold);
        XFont font9 = new XFont("Verdana", 9, XFontStyle.Bold);
        XFont font20Regular = new XFont("Verdana", 20, XFontStyle.Regular);
        XFont font18Regular = new XFont("Verdana", 18, XFontStyle.Regular);
        XFont font17Regular = new XFont("Verdana", 17, XFontStyle.Regular);
        XFont font6Regular = new XFont("Verdana", 6, XFontStyle.Regular);
        XFont font5Regular = new XFont("Verdana", 5, XFontStyle.Regular);
        XFont font8Regular = new XFont("Verdana", 8, XFontStyle.Regular);
        XFont font10Regular = new XFont("Verdana", 10, XFontStyle.Regular);
        void DrawPageNumberText(XGraphics gfx, int startX, int startY, string pageNo, string side)
        {
            gfx.DrawString(pageNo, font18, XBrushes.White, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX).Point, (float)XUnit.FromMillimeter(startY).Point));

        }
        void DrawSerieRow(XGraphics gfx, int startX, int startY, string serie)
        {
            Pen pen = new Pen(Color.FromArgb(0,0,0), 1);
            pen.Alignment = PenAlignment.Inset; //<-- this

            gfx.DrawRectangle(pen, XBrushes.White, new XRect(XUnit.FromMillimeter(startX + 14.1), XUnit.FromMillimeter(startY + 30), XUnit.FromMillimeter(188.8), XUnit.FromMillimeter(10)));
            gfx.DrawString(serie, font9, new XSolidBrush(XColor.FromArgb(0,0,0)), new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 20).Point, (float)XUnit.FromMillimeter(startY + 36).Point));

        }
        void DrawBrandRow(XGraphics gfx, int startX, int startY, string brand)
        {
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(186, 219, 238)), new XRect(XUnit.FromMillimeter(startX + 13.9), XUnit.FromMillimeter(startY + 30), XUnit.FromMillimeter(189.2), XUnit.FromMillimeter(10)));
            gfx.DrawString(brand, font10, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 20).Point, (float)XUnit.FromMillimeter(startY + 36).Point));

        }
        void DrawLineLayout(XGraphics gfx, int startX, int startY, int rowCount, int heightSum)
        {
            gfx.DrawLine(new XPen(XColor.FromArgb(0,0,0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 14), XUnit.FromMillimeter(30 + heightSum)), new XPoint(XUnit.FromMillimeter(startX + 14 + 188), XUnit.FromMillimeter(30 + heightSum)));
            gfx.DrawLine(new XPen(XColor.FromArgb(0,0,0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 14), XUnit.FromMillimeter(30 + rowCount * 5 + heightSum)), new XPoint(XUnit.FromMillimeter(startX + 14 + 188.5), XUnit.FromMillimeter(30 + rowCount * 5 + heightSum)));
        }
        void DrawLineLayoutForModels(XGraphics gfx, int startX, int startY, int brandNumber, int serieNumber)
        {
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(186, 219, 238)), new XRect(XUnit.FromMillimeter(startX + 151), XUnit.FromMillimeter(startY + 50 + serieNumber * 10 + brandNumber * 20), XUnit.FromMillimeter(13), XUnit.FromMillimeter(5)));

            gfx.DrawLine(new XPen(XColor.FromArgb(0,0,0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 14), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 14), XUnit.FromMillimeter(55 + startY + serieNumber * 10 + brandNumber * 20)));
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 14 + 189), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 14 + 189), XUnit.FromMillimeter(55 + startY + serieNumber * 10 + brandNumber * 20)));
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 143.5), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 15 + 188), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)));
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 143.5), XUnit.FromMillimeter(55 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 15 + 188), XUnit.FromMillimeter(55 + startY + serieNumber * 10 + brandNumber * 20)));

            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 64), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 64), XUnit.FromMillimeter(startY + 55 + serieNumber * 10 + brandNumber * 20)));
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 127.5), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 127.5), XUnit.FromMillimeter(startY + 55 + serieNumber * 10 + brandNumber * 20)));

            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 143.5), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 143.5), XUnit.FromMillimeter(startY + 55 + serieNumber * 10 + brandNumber * 20)));

            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 151), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 151), XUnit.FromMillimeter(startY + 55 + serieNumber * 10 + brandNumber * 20)));

            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 164), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 164), XUnit.FromMillimeter(startY + 55 + serieNumber * 10 + brandNumber * 20)));
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 170), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 170), XUnit.FromMillimeter(startY + 55 + serieNumber * 10 + brandNumber * 20)));
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0), 0.5), new XPoint(XUnit.FromMillimeter(startX + 180.25), XUnit.FromMillimeter(50 + startY + serieNumber * 10 + brandNumber * 20)), new XPoint(XUnit.FromMillimeter(startX + 180.25), XUnit.FromMillimeter(startY + 55 + serieNumber * 10 + brandNumber * 20)));
        }
        void DrawLineText(XGraphics gfx, int startX, int startY, CvlItem dataLine, int heightSum, int rowCount)
        {
            if (dataLine.Model.Count() > 39)
            {
                gfx.DrawString(dataLine.Model.Replace(",5\"", ",5''").Replace(".5\"", ".5''").Replace("\"", ""), font5, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 15).Point, (float)XUnit.FromMillimeter(31 + heightSum + ((rowCount * 5) / 2)).Point));
            }
            else
            {
                gfx.DrawString(dataLine.Model.Replace(",5\"", ",5''").Replace(".5\"", ".5''").Replace("\"", ""), font6, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 15).Point, (float)XUnit.FromMillimeter(31 + heightSum + ((rowCount * 5) / 2)).Point));

            }
            gfx.DrawString(dataLine.WvaDesc, font6Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 65).Point, (float)XUnit.FromMillimeter(31 + heightSum + ((rowCount * 5) / 2)).Point));
            
            //else if (!string.IsNullOrEmpty(dataLine[3]) && string.IsNullOrEmpty(dataLine[4]) || dataLine[4] == " ")
            //{
            //    gfx.DrawString(dataLine[3].Replace("10\"", "10''").Replace(".5\"", ".5''").Replace("\"", ""), font6Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 65).Point, (float)XUnit.FromMillimeter(31 + heightSum + ((rowCount * 5) / 2)).Point));
            //}
            //else if (!string.IsNullOrEmpty(dataLine[4]) && (string.IsNullOrEmpty(dataLine[3]) || dataLine[3] == " "))
            //{
            //    gfx.DrawString(dataLine[4].Replace("10\"", "10''").Replace(".5\"", ".5''").Replace("\"", ""), font6Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 65).Point, (float)XUnit.FromMillimeter(31 + heightSum + ((rowCount * 5) / 2)).Point));
            //}
            gfx.DrawString(GetYears(dataLine.DateFrom, dataLine.DateTo), font6Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 128).Point, (float)XUnit.FromMillimeter(31 + heightSum + ((rowCount * 5) / 2)).Point));
        }
        void DrawLineTextForModels(XGraphics gfx, int startX, int startY, CvlItem dataLine, int heightSum, int rowCount, int brandNumber, int serieNumber)
        {
            gfx.DrawString(dataLine.Fr, font5Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 146.5).Point, (float)XUnit.FromMillimeter(51 + startY + serieNumber * 10 + brandNumber * 20).Point));
            gfx.DrawString(dataLine.BadBeeNumber, font5, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 151.5).Point, (float)XUnit.FromMillimeter(51 + startY + serieNumber * 10 + brandNumber * 20).Point));
            gfx.DrawString(dataLine.BrakeSystem, font5Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 166).Point, (float)XUnit.FromMillimeter(51 + startY + serieNumber * 10 + brandNumber * 20).Point));
            gfx.DrawString(dataLine.Wva, font5Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 171).Point, (float)XUnit.FromMillimeter(51 + startY + serieNumber * 10 + brandNumber * 20).Point));
            gfx.DrawString(dataLine.Size, font5Regular, XBrushes.Black, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 181).Point, (float)XUnit.FromMillimeter(51 + startY + serieNumber * 10 + brandNumber * 20).Point));

        }
        void DrawImage(XGraphics gfx, string path, int x, int y, int width, int height)
        {
            XImage image = XImage.FromFile(path);
            gfx.DrawImage(image, x, y, width, height);
        }
        void DrawImage2(XGraphics gfx, Image gdiPlusimage, int x, int y, int width, int height)
        {
            XImage image = XImage.FromGdiPlusImage(gdiPlusimage);
            gfx.DrawImage(image, x, y, width, height);
        }
        int AddDataPages(XGraphics gfx, List<CvlItem> data, PdfPage page, PdfDocument document, int maxHeight, int maxWidth)
        {
            for (int pageNo = 1; pageNo < data.Count()/30; pageNo++)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                DrawImage2(gfx, PDFResources.Header, 40, 20, maxWidth - 60, 50);

                if (pageNo % 2 == 0)
                {
                    DrawImage2(gfx, PDFResources.Page_number_row_left, 0, maxHeight - 40, maxWidth, 25);
                    DrawPageNumberText(gfx, 4, 291, pageNo.ToString(), "left");
                }
                else
                {
                    DrawImage2(gfx, PDFResources.Page_number_row_right, 0, maxHeight - 40, maxWidth, 25);
                    DrawPageNumberText(gfx, 194, 291, pageNo.ToString(), "right");
                }

                var testData = data.ToList();
                testData = data.Skip((pageNo - 1) * 30).Take(30).ToList();
                var brands = testData.GroupBy(q => q.Brand).Select(q => q.First()).ToList();


                string brand = "";
                string serie = "";
                string model = "";
                string text1 = "";
                string text2 = "";
                string date = "";
                int serieNumber = 0;
                var heightSum = 0;
                var index = 0;
                for (int l = 0; l < brands.Count(); l++)
                {
                    brand = brands.ElementAt(l).Brand;
                    var series = testData.Where(q => q.Brand == brand).GroupBy(q => q.Serie).Select(q => q.First()).ToList();

                    DrawBrandRow(gfx, 0, heightSum, brand);
                    serie = series.ElementAt(0).Serie;
                    heightSum = +heightSum + 10;

                    for (int k = 0; k < series.Count(); k++)
                    {
                        serie = series.ElementAt(k).Serie;
                        var models = testData.Where(q => q.Serie == serie).Where(q => q.Brand == brand).GroupBy(q => new { Column1 = q.Model, Column2 = q.Years}).Select(q => q.First()).ToList();

                        if (serie == "")
                        {
                            for (int i = 1; i < models.Count + 1; i++)
                            {
                                model = models.ElementAt(i - 1).Model;
                                date = models.ElementAt(i - 1).Years;
                                var listOfItemsInModel = testData.Where(q => q.Years == date).Where(q => q.Model == model).Where(q => q.Serie == serie).Where(q => q.Brand == brand).ToList();
                                int ItemsInModelCount = listOfItemsInModel.Count();

                                DrawLineLayout(gfx, 0, i, ItemsInModelCount, heightSum);
                                DrawLineText(gfx, 0, i + 2, models[i - 1], heightSum, ItemsInModelCount);


                                for (int j = 1; j < listOfItemsInModel.Count() + 1; j++)
                                {
                                    DrawLineLayoutForModels(gfx, 0, index * 5, l, serieNumber - 1);
                                    DrawLineTextForModels(gfx, 0, index * 5 + 2, listOfItemsInModel.ElementAt(j - 1), heightSum, ItemsInModelCount, l, serieNumber - 1);

                                    index++;
                                }

                                heightSum += ItemsInModelCount * 5;
                            }
                        }
                        else
                        {

                            DrawSerieRow(gfx, 0, heightSum, serie);
                            heightSum = +heightSum + 10;
                            for (int i = 1; i < models.Count + 1; i++)
                            {
                                model = models.ElementAt(i - 1).Model;
                                //text1 = models.ElementAt(i - 1)[3];
                                //text2 = models.ElementAt(i - 1)[4];
                                date = models.ElementAt(i - 1).Years;
                                var listOfItemsInModel = testData.Where(q => q.Years == date).Where(q => q.Model == model).Where(q => q.Serie == serie).Where(q => q.Brand  == brand).ToList();

                                int ItemsInModelCount = listOfItemsInModel.Count();

                                DrawLineLayout(gfx, 0, i, ItemsInModelCount, heightSum);
                                DrawLineText(gfx, 0, i + 2, models[i - 1], heightSum, ItemsInModelCount);


                                for (int j = 1; j < listOfItemsInModel.Count() + 1; j++)
                                {
                                    DrawLineLayoutForModels(gfx, 0, index * 5, l, serieNumber);
                                    DrawLineTextForModels(gfx, 0, index * 5 + 2, listOfItemsInModel.ElementAt(j - 1), heightSum, ItemsInModelCount, l, serieNumber);
                                    index++;
                                }

                                heightSum += ItemsInModelCount * 5;
                            }
                            serieNumber++;
                        }

                    }
                        heightSum = heightSum + 10;
                    }
            }
            gfx.Dispose();
            return 119;
        }
        int AddPicturePages(XGraphics gfx, PdfPage page, PdfDocument document, int maxHeight, int maxWidth, int pageNo)
        {
            List<string[]> data = new List<string[]>();
            //foreach (var dataLine in Properties.Resources.PicturesDetails.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    data.Add(dataLine.Split(';'));
            //}
            var testData = data.ToList();
            string dbNumber = "";
            string wva = "";
            string applications = "";
            string backgroundPath = "../../Resources/details_page.png";

            //for (int i = 1; i < 5; i++) 
            for (int i = 1; i < testData.Count; i++)
            {

                dbNumber = testData.ElementAt(i)[0];
                wva = testData.ElementAt(i)[1];
                applications = testData.ElementAt(i)[3];

                string path = "../../Resources/" + dbNumber.Replace("DB ", "") + ".jpg";

                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                DrawDetailsBackground(gfx, 0, 0, maxWidth, maxHeight);
                DrawImage(gfx, path, 15, 75, 550, 660);
                if (i % 2 == 0)
                {
           //         DrawImage2(gfx, Properties.Resources.Page_number_row_left, 0, maxHeight - 40, maxWidth, 25);
                    DrawPageNumberText(gfx, 4, 291, (i + 122).ToString(), "left");
                }
                else
                {
           //         DrawImage2(gfx, Properties.Resources.Page_number_row_right, 0, maxHeight - 40, maxWidth, 25);
                    DrawPageNumberText(gfx, 194, 291, (i + 122).ToString(), "right");
                }

                DrawDBNumber(gfx, 0, 0, dbNumber);
                DrawWvaNumber(gfx, 0, 0, wva);


                DrawApplications(gfx, 0, 0, applications);

                gfx.Dispose();
            }
            return pageNo + 122;

        }
        void DrawDetailsBackground(XGraphics gfx, int startX, int startY, int maxWidth, int maxHeight)
        {
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(186, 219, 238)), new XRect(XUnit.FromMillimeter(startX), XUnit.FromMillimeter(startY + 3), XUnit.FromMillimeter(maxWidth), XUnit.FromMillimeter(startY + 10)));
        }
        void DrawDBNumber(XGraphics gfx, int startX, int startY, string dbNumber)
        {
            gfx.DrawString(dbNumber, font18, XBrushes.White, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 20).Point, (float)XUnit.FromMillimeter(startY + 10).Point));

        }
        void DrawApplications(XGraphics gfx, int startX, int startY, string applications)
        {
            XTextFormatter tf = new XTextFormatter(gfx);

            XRect rect = new XRect(50, 50, 500, 30);

            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(applications, font10Regular, XBrushes.Black, rect, XStringFormats.TopLeft);


        }
        void DrawWvaNumber(XGraphics gfx, int startX, int startY, string wva)
        {
            gfx.DrawString("WVA " + wva, font18, XBrushes.White, new System.Drawing.PointF((float)XUnit.FromMillimeter(startX + 150).Point, (float)XUnit.FromMillimeter(startY + 9.65).Point));

        }
    }
}
