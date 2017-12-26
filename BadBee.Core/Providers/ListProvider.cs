using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics;
using BadBee.Core.Models;
using BadBee.Core.DAL;

namespace BadBee.Core.Providers
{
    public class ListProvider : IDisposable
    {
        private BadBeeEntities db;
        private bool IsDisposed;

        public ListProvider()
        {
            db = new BadBeeEntities();
        }

        public class ListResult
        {
            public List<CvlItem> Items { get; set; }
            public int ItemsCount { get; set; }
        }

        public ListResult GetList(BadBeeFilter filter, int page, int itemsPerPage)

        {
            ListResult results = new ListResult();
            results.Items = new List<CvlItem>();
            results.ItemsCount = 0;

            var query = db.Item.AsQueryable();

            if (filter.PhraseFilter != null)
            {
                if (GetChList<Brand>().Any(row => row.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())))
                {
                    List<Brand> br = GetChList<Brand>().Where(q => q.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())).ToList();
                    List<string> brandIds = br.Select(x => x.BrandId.ToString()).ToList();
                    query = query.Where(q => brandIds.Contains(q.Model.Serie.Brand.BrandId.ToString()));
                    filter.BrandsList = brandIds;
                }
                else if (GetChList<Serie>().Any(row => row.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())))
                {
                    List<Serie> ser = GetChList<Serie>().Where(q => q.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())).ToList();
                    List<string> serieIds = ser.Select(x => x.SerieId.ToString()).ToList();
                    query = query.Where(q => serieIds.Contains(q.Model.Serie.SerieId.ToString()));
                    filter.SeriesList = serieIds;
                }
                else if (GetChList<Model>().Any(row => row.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())))
                {
                    List<Model> mod = GetChList<Model>().Where(q => q.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())).ToList();
                    List<string> modelIds = mod.Select(x => x.ModelId.ToString()).ToList();
                    query = query.Where(q => modelIds.Contains(q.ModelId.ToString()));
                    filter.ModelsList = modelIds;
                }
                else
                     if (GetChList<DAL.BadBee>().Any(q =>
                    (q.BadBeeNo.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                    .StartsWith(filter.PhraseFilter.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")))))
                {
                    List<DAL.BadBee> lum = GetChList<DAL.BadBee>().Where(q => (q.BadBeeNo.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                        .StartsWith(filter.PhraseFilter.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")))).ToList();

                    List<string> badBeeIds = lum.Select(x => x.BadBeeId.ToString()).ToList();
                    query = query.Where(q => badBeeIds.Contains(q.BadBeeId.ToString()));
                    if (lum != null)
                    {
                        filter.BadBeeNumbersList = badBeeIds;
                    }
                }
                else if (GetChList<Wva>().Any(q => q.WvaNo.StartsWith(filter.PhraseFilter.Replace(" ", ""))))
                {
                    List<Wva> wva = GetChList<Wva>().Where(q => q.WvaNo.Replace(" ", "").StartsWith(filter.PhraseFilter.Replace(" ", ""))).ToList();

                    List<string> wvaIds = wva.Select(x => x.WvaId.ToString()).ToList();
                    query = query.Where(q => wvaIds.Contains(q.BadBee.WvaId.ToString()));
                    if (wva != null)
                    {
                        filter.WvasList = wvaIds;
                    }
                }
                filter.PhraseFilter = null;
                return results;
            }

            if (filter != null && filter.PhraseFilter == null)
            {
                if (!string.IsNullOrEmpty(filter.Models))
                {
                    query = query.Where(q => filter.ModelsList.Contains(q.ModelId.ToString()));
                }

                else if (!string.IsNullOrEmpty(filter.Series))
                {
                    query = query.Where(q => filter.SeriesList.Contains(q.Model.Serie.SerieId.ToString()));
                }
                else if (!string.IsNullOrEmpty(filter.Brands))
                {
                    query = query.Where(q => filter.BrandsList.Contains(q.Model.Serie.Brand.BrandId.ToString()));
                }
                if (string.IsNullOrEmpty(filter.Brands))
                {
                    filter.Series = null;
                    filter.Models = null;
                }
                if (!string.IsNullOrEmpty(filter.BadBeeNumbers))
                {
                    query = query.Where(q => filter.BadBeeNumbersList.Contains(q.BadBee.BadBeeId.ToString()));
                }
                //if (filter.DateYear.HasValue)
                //{
                //    DateTime date = new DateTime();
                //    date = date.AddYears(filter.DateYear.Value - 1);

                //    query = query.Where(q => (q.DateFrom <= date && q.DateTo >= date) || (q.DateFrom <= date && q.DateTo == null) || (q.DateFrom == null && q.DateTo >= date));
                //}
                if (!string.IsNullOrEmpty(filter.DateYears))
                {
                    // DateTime date = new DateTime();
                    List<int> yslist = filter.DateYearsList.Select(int.Parse).ToList();

                    query = query.Where(q => yslist.Contains(q.Model.Year.DateFrom.Value.Year) || yslist.Contains(q.Model.Year.DateTo.Value.Year));
                }
                if (!string.IsNullOrEmpty(filter.Wvas))
                {
                    query = query.Where(q => filter.WvasList.Contains(q.BadBee.Wva.WvaId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Widths))
                {
                    query = query.Where(q => filter.WidthsList.Contains(q.BadBee.Dimension.Width.WidthId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Heights))
                {
                    query = query.Where(q => filter.HeightsList.Contains(q.BadBee.Dimension.Height.HeightId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Thicknesses))
                {
                    query = query.Where(q => filter.ThicknessesList.Contains(q.BadBee.Dimension.Thickness.ThicknessId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Systems))
                {
                    query = query.Where(q => filter.SystemsList.Contains(q.BadBee.Systems.SystemId.ToString()));
                }

                query = query.OrderBy(q => q.Model.Serie.Brand.Name).ThenBy(q => q.Model.Serie.Name).ThenBy(q => q.Model.Name).ThenBy(q => q.Id);
                results.ItemsCount = query.Count();
                query = query.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
                var list = query.ToList();
                filter.PhraseFilter = null;

                foreach (Item item in list)
                {
                    string sYears = "";

                    if (item.Model.Year.DateFrom.HasValue)
                    {
                        sYears += item.Model.Year.DateFrom.Value.ToString("MM/yy", CultureInfo.InvariantCulture);

                        if (item.Model.Year.DateTo.HasValue)
                        {
                            sYears += " - " + item.Model.Year.DateTo.Value.ToString("MM/yy", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            sYears += " ->";
                        }
                    }
                    else
                    {
                        if (item.Model.Year.DateTo.HasValue)
                        {
                            sYears += "<- " + item.Model.Year.DateTo.Value.ToString("MM/yy", CultureInfo.InvariantCulture);
                        }
                    }


                    CvlItem newItem = new CvlItem()
                    {
                        Id = item.Id,
                        BadBeeNumber = item.BadBee.BadBeeNo.TrimEnd(),
                        Brand = item.Model.Serie.Brand.Name.TrimEnd(),
                        Serie = item.Model.Serie.Name.TrimEnd(),
                        Model = item.Model.Name.TrimEnd(),
                        Years = sYears.TrimEnd(),
                        Fr = item.BadBee.FR.TrimEnd(),
                        WvaDesc = item.BadBee.Wva.Description.TrimEnd(),
                        Wva = item.BadBee.Wva.WvaNo.TrimEnd(),
                        Width = item.BadBee.Dimension.Width.Width1.ToString(),
                        Height = item.BadBee.Dimension.Height.Height1.ToString(),
                        Thickness = item.BadBee.Dimension.Thickness.Thickness1.ToString(),
                        BrakeSystem = item.BadBee.Systems.Abbreviation.TrimEnd()
                    };
                    results.Items.Add(newItem);
                }
            }
            return results;
            
        }

        public static void FillDictionaryCache()
        {
            using (BadBeeEntities db = new BadBeeEntities())
            {
                GlobalVars.DictionaryCache.Add(typeof(Brand), db.Brand.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Serie), db.Serie.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Model), db.Model.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(DAL.BadBee), db.BadBee.OrderBy(q => q.BadBeeNo).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Wva), db.Wva.OrderBy(q => q.WvaNo).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Systems), db.Systems.OrderBy(q => q.Abbreviation).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Width), db.Width.OrderBy(q => q.Width1).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Height), db.Height.OrderBy(q => q.Height1).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Thickness), db.Thickness.OrderBy(q => q.Thickness1).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Year), db.Year.OrderBy(q => q.DateFrom).ToList());
               }
        }

        public List<T> GetChList<T>()
        {
            return GlobalVars.DictionaryCache[typeof(T)] as List<T>;
        }
        public List<Brand> GetBrandsChList(BadBeeFilter filter)
        {
            return GetChList<Brand>().Where(q => filter.BrandsList.Contains(q.BrandId.ToString())).OrderBy(q => q.Name).ToList();
        }
        public List<Serie> GetSeriesChList(BadBeeFilter filter)
        {
            return GetChList<Serie>().Where(q => filter.SeriesList.Contains(q.SerieId.ToString())).ToList();
        }
        public List<Model> GetModelsChList(BadBeeFilter filter)
        {
            return GetChList<Model>().Where(q => filter.ModelsList.Contains(q.ModelId.ToString())).ToList();
        }
        public List<DAL.BadBee> GetBadBeeChList(BadBeeFilter filter)
        {
            return GetChList<DAL.BadBee>().Where(q => (filter.BadBeeNumbersList.Contains(q.BadBeeId.ToString()))).ToList();
         }
        public List<Wva> GetWvaChList(BadBeeFilter filter)
        {
            return GetChList<Wva>().Where(q => filter.WvasList.Contains(q.WvaId.ToString())).ToList();
        }
        public List<Systems> GetSystemsListCh(BadBeeFilter filter)
        {
            return GetChList<Systems>().Where(q => filter.SystemsList.Contains(q.SystemId.ToString())).ToList();
        }
        public List<Width> GetWidthsListCh(BadBeeFilter filter)
        {
            return GetChList<Width>().Where(q => filter.WidthsList.Contains(q.WidthId.ToString())).ToList();
        }
        public List<Thickness> GetThicknessesListCh(BadBeeFilter filter)
        {
            return GetChList<Thickness>().Where(q => filter.ThicknessesList.Contains(q.ThicknessId.ToString())).ToList();
        }
        public List<Height> GetHeightsListCh(BadBeeFilter filter)
        {
            return GetChList<Height>().Where(q => filter.HeightsList.Contains(q.HeightId.ToString())).ToList();
        }
        public List<Year> GetYearsListCh(BadBeeFilter filter)
        {
            return GetChList<Year>().Where(q => filter.DateYearsList.Contains(q.YearId.ToString())).ToList();
        }
       

        public List<Item> GetList(BadBeeFilter filter, bool brand, bool serie, bool model, bool drum, bool height, bool width, 
            bool thick, bool system, bool rivet, bool wva, bool wvad, bool lum, bool year)
        {
            if (string.IsNullOrEmpty(GlobalVars.SearchCache.SearchKey) == false && GlobalVars.SearchCache.SearchKey.Equals(filter.SearchKey))
            {
                Debug.WriteLine(DateTime.Now + " Cache search with key " + GlobalVars.SearchCache.SearchKey);

                return GlobalVars.SearchCache.GetListResult;
            }
            else
            {
                var items = db.Item.AsQueryable();

                if (!string.IsNullOrEmpty(filter.Models) && !model)
                {
                    items = items.Where(q => filter.ModelsList.Contains(q.ModelId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Series) && !serie)
                {
                    items = items.Where(q => filter.SeriesList.Contains(q.Model.SerieId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Brands) && !brand)
                {
                    items = items.Where(q => filter.BrandsList.Contains(q.Model.Serie.BrandId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Heights) && !height)
                {
                    items = items.Where(q => filter.HeightsList.Contains(q.BadBee.Dimension.Height.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Widths) && !width)
                {
                    items = items.Where(q => filter.WidthsList.Contains(q.BadBee.Dimension.Width.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Thicknesses) && !thick)
                {
                    items = items.Where(q => filter.ThicknessesList.Contains(q.BadBee.Dimension.Thickness.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Systems) && !system)
                {
                    items = items.Where(q => filter.SystemsList.Contains(q.BadBee.SystemId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Wvas) && !wva)
                {
                    items = items.Where(q => filter.WvasList.Contains(q.BadBee.WvaId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.BadBeeNumbers) && !lum)
                {
                    items = items.Where(q => filter.BadBeeNumbersList.Contains(q.BadBeeId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.DateYears) && !year)
                {
                    items = items.Where(q => filter.DateYearsList.Contains(q.Model.Year.DateFrom.Value.Year.ToString()) || filter.DateYearsList.Contains(q.Model.Year.DateTo.Value.Year.ToString()));
                }

                var searchCache = new SearchCache();
                //searchCache.GetListResult = items.ToList();
                searchCache.SearchKey = filter.SearchKey;

                GlobalVars.SearchCache = searchCache;

                Debug.WriteLine(DateTime.Now + " New search with key " + GlobalVars.SearchCache.SearchKey);

                return GlobalVars.SearchCache.GetListResult;
            }
        }
        public List<Brand> GetBrandsList(BadBeeFilter filter)
        {
            //if (string.IsNullOrEmpty(filter.CrossNumbers) && /*string.IsNullOrEmpty(filter.Brands) && */string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
            //        && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
            //        && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
            //        && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
            //        && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            //{
                return GetChList<Brand>();
            }

        //    else
        //    {
        //        var items = GetList(filter, true, false, false, false, false, false, false, false, false, false, false, false, false);
        //        var list = items.ToList();
        //        var lum = list.Select(item => new Brand { BrandId = item. ?? default(int), Name = item.Brand.TrimEnd() }).ToList();
        //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

        //        return distinctRowsById.OrderBy(q => q.Name).ToList();
        //    }

        //}
        public List<DAL.BadBee> GetBadBeeList(BadBeeFilter filter)
        {
        //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
        //            && string.IsNullOrEmpty(filter.DateYears) /*&& string.IsNullOrEmpty(filter.BadBeeNumbers)*/ && string.IsNullOrEmpty(filter.Wvas)
        //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
        //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
        //            && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
        //    {
               return GetChList<DAL.BadBee>();
        //    }
        //    else
        //    {
        //        var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, false, true, false);
        //        var list = items.OrderBy(q => q.BadBeeNumber).ToList();
        //        var lum = list.Select(item => new BadBeeNumbers { BadBeeNumber = item.BadBeeNumber.TrimEnd(), BadBeeNumberId = item.BadBeeNumberId ?? default(int) }).ToList();
        //        var distinctRowsById = lum.Select(i => i.BadBeeNumberId).Distinct().Select(i => lum.First(o => o.BadBeeNumberId == i)).ToList();

        //        return distinctRowsById.OrderBy(q => q.BadBeeNumber).ToList();
        //    }
        }
        //public List<string> GetPadsWvasList()
        //{
        //    var items = db.ItemsDb.Where(q => q.ProductType == "Brake pad").ToList();
        //    List<Wva> wva = items.Distinct().Where(q => q.WvaId != 0).Select(item => new Wva { Id = item.WvaId ?? default(int), Name = item.Wva }).ToList();
        //    var distinctWvasRowsById = wva.Select(i => i.Id).Distinct().Select(i => wva.First(o => o.Id == i)).ToList();
        //    List<string> wvaNames = distinctWvasRowsById.Select(s => s.Name).ToList();
        //    return wvaNames.OrderBy(q => q).ToList();
        //}
        //public List<string> GetPadsBadBeesList()
        //{
        //    var items = db.ItemsDb.Where(q => q.ProductType == "Brake pad").ToList();
        //    List<BadBeeNumbers> badBee = items.Distinct().Where(q => q.BadBeeNumberId != 0).Select(item => new BadBeeNumbers { BadBeeNumberId = item.BadBeeNumberId ?? default(int), BadBeeNumber = item.BadBeeNumber }).ToList();
        //    var distinctBadBeesRowsById = badBee.Select(i => i.BadBeeNumberId).Distinct().Select(i => badBee.First(o => o.BadBeeNumberId == i)).ToList();
        //    List<string> badBeeNames = distinctBadBeesRowsById.Select(s => s.BadBeeNumber).ToList();
        //    return badBeeNames.OrderBy(q => q).ToList(); ;
        //}
        public List<Wva> GetWvaList(BadBeeFilter filter)
        {
        //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
        //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) /*&& string.IsNullOrEmpty(filter.Wvas)*/
        //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
        //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
        //            && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
        //    {
                return GetChList<Wva>();
        //    }
        //    else
        //    {
        //        var items = GetList(filter, false, false, false, false, false, false, false, false, false, true, false, false, false);

        //        var list = items.OrderBy(q => q.Wva).ToList();
        //        var lum = list.Distinct().Select(item => new Wva { Id = item.WvaId ?? default(int), Name = item.Wva.TrimEnd() }).ToList();
        //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

        //        return distinctRowsById.OrderBy(q => q.Name).ToList();
        //    }
        }

        //public List<WvaDetails> GetWvaDetList(BadBeeFilter filter)
        //{
        //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
        //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
        //           /* && string.IsNullOrEmpty(filter.WvaDetails2)*/ && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
        //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
        //            && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
        //    {
        //        return GetChList<WvaDetails>();
        //    }
        //    else
        //    {
        //        var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, true, false, false);
        //        var list = items.OrderBy(q => q.WvaDetails).ToList();
        //        var lum = list.Select(item => new WvaDetails { Id = item.WvaDetailsId ?? default(int), Name = item.WvaDetails.TrimEnd() }).ToList();
        //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

        //        return distinctRowsById.OrderBy(q => q.Name).ToList();
        //    }
        //}
        public List<Year> GetYearsList(BadBeeFilter filter)
        {
        //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
        //            /*&& string.IsNullOrEmpty(filter.DateYears)*/ && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
        //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
        //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
        //            && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
        //    {
                return GetChList<Year>();
        //    }
        //    else
        //    {
        //        var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, false, false, true);
        //        var list = items.ToList();
        //        var lum1 = list.Distinct().Select(item => new Years { Id = item.DateFrom.HasValue ? item.DateFrom.Value.Year : 0, Name = item.DateFrom.HasValue ? item.DateFrom.Value.Year.ToString() : string.Empty }).ToList();
        //        var lum2 = list.Distinct().Select(item => new Years { Id = item.DateTo.HasValue ? item.DateTo.Value.Year : 0, Name = item.DateTo.HasValue ? item.DateTo.Value.Year.ToString() : string.Empty }).ToList();
        //        var lum = lum1.Concat(lum2).ToList();
        //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

        //        return distinctRowsById.OrderBy(q => q.Name).ToList();
        //    }
        }
        public List<Height> GetHeightsList(BadBeeFilter filter)
        {
        //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
        //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
        //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
        //            && string.IsNullOrEmpty(filter.Widths)/* && string.IsNullOrEmpty(filter.Heights) */&& string.IsNullOrEmpty(filter.Thicknesses)
        //            && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
        //    {
               return GetChList<Height>();
        //    }
        //    else
        //    {
        //        var items = GetList(filter, false, false, false, false, true, false, false, false, false, false, false, false, false);
        //        var list = items.ToList();
        //        var lum = list.Select(item => new Heights { Id = item.HeightId ?? default(int), Name = item.Height.TrimEnd() }).ToList();
        //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

        //        return distinctRowsById.OrderBy(q => q.Name).ToList();
        //    }
        }
        public List<Width> GetWidthsList(BadBeeFilter filter)
        {
            //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
            //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
            //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
            //            /*&& string.IsNullOrEmpty(filter.Widths)*/ && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
            //            && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            //    {
                    return GetChList<Width>();
            //    }
            //    else
            //    {
            //        var items = GetList(filter, false, false, false, false, false, true, false, false, false, false, false, false, false);
            //        var list = items.ToList();
            //        var lum = list.Select(item => new Widths { Id = item.WidthId ?? default(int), Name = item.Width.TrimEnd() }).ToList();
            //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

            //        return distinctRowsById.OrderBy(q => q.Name).ToList();
            //    }
            }
            public List<Thickness> GetThicknessesList(BadBeeFilter filter)
            {
            //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
            //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
            //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
            //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) /*&& string.IsNullOrEmpty(filter.Thicknesses)*/
            //            && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            //    {
                    return GetChList<Thickness>();
            //    }
            //    else
            //    {
            //        var items = GetList(filter, false, false, false, false, false, false, true, false, false, false, false, false, false);
            //        var list = items.ToList();
            //        var lum = list.Select(item => new Thicknesses { Id = item.ThicknessId ?? default(int), Name = item.Thickness.TrimEnd() }).ToList();
            //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

            //        return distinctRowsById.OrderBy(q => q.Name).ToList();
            //    }
            }
            //public List<DrumDiameters> GetDrumDiametersList(BadBeeFilter filter)
            //{
            //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
            //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
            //            && string.IsNullOrEmpty(filter.WvaDetails2) /*&& string.IsNullOrEmpty(filter.DrumDiameters)*/ && string.IsNullOrEmpty(filter.Rivets)
            //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
            //            && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            //    {
            //        return GetChList<DrumDiameters>();
            //    }
            //    else
            //    {
            //        var items = GetList(filter, false, false, false, true, false, false, false, false, false, false, false, false, false);
            //        var list = items.ToList();
            //        var lum = list.Select(item => new DrumDiameters { Id = item.DrumDiameterId ?? default(int), Name = item.DrumDiameter.TrimEnd() }).ToList();
            //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

            //        return distinctRowsById.OrderBy(q => q.Name).ToList();
            //    }
            //}
            public List<Systems> GetSystemsList(BadBeeFilter filter)
            {
            //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
            //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
            //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
            //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
            //            /*&& string.IsNullOrEmpty(filter.Systems)*/ /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            //    {
                    return GetChList<Systems>();
            //    }
            //    else
            //    {
            //        var items = GetList(filter, false, false, false, false, false, false, false, true, false, false, false, false, false);
            //        var list = items.ToList();
            //        var lum = list.Select(item => new Systems { Id = item.SystemId ?? default(int), Name = item.BrakeSystem.TrimEnd() }).ToList();
            //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

            //        return distinctRowsById.OrderBy(q => q.Name).ToList();
            //    }
            }
            //public List<RivetTypes> GetRivetTypesList(BadBeeFilter filter)
            //{
            //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
            //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
            //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) /*&& string.IsNullOrEmpty(filter.Rivets)*/
            //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
            //            && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            //    {
            //        return GetChList<RivetTypes>().OrderBy(q => q.Id).ToList();
            //    }
            //    else
            //    {
            //        var items = GetList(filter, false, false, false, false, false, false, false, false, true, false, false, false, false);
            //        var list = items.ToList();
            //        var lum = list.Select(item => new RivetTypes { Id = item.RivetTypeId ?? default(int), Name = item.RivetsType.TrimEnd() }).ToList();
            //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

            //        return distinctRowsById.OrderBy(q => q.Name).ToList();
            //    }
            //}
            public List<Serie> GetSeriesList(BadBeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) /*&& string.IsNullOrEmpty(filter.Series)*/ && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Serie>().OrderBy(q => q.Name).ToList();
            }
            else
            {
                var items = GetList(filter, false, true, false, false, false, false, false, false, false, false, false, false, false);
            var list = items.ToList();
            var bb = list.Select(x => new Serie() { SerieId = x.Model.Serie.SerieId, Name=x.Model.Serie.Name.TrimEnd() }).ToList();
            var distinctRowsById = bb.Select(i => i.SerieId).Distinct().Select(i => bb.First(o => o.SerieId == i)).ToList();

            return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<Model> GetModelsList(BadBeeFilter filter)
        {
            //    if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series)/* && string.IsNullOrEmpty(filter.Models)*/
            //            && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
            //            && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
            //            && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
            //            && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            //    {
                   return GetChList<Model>();
            //    }
            //    else
            //    {
            //        var items = GetList(filter, false, false, true, false, false, false, false, false, false, false, false, false, false);
            //        var list = items.ToList();
            //        var lum = list.Select(item => new DAL.Models { Id = item.ModelId ?? default(int), Name = item.Model.TrimEnd() }).ToList();

            //        var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

            //        return distinctRowsById;
            //    }
            }

            public List<string> GetKeywords(string keywordPart)
        {
            return db.GetKeywords(keywordPart).OrderBy(q => q).Take(5).ToList();
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed) db.Dispose();
            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static int GetAllRecordsCount()
        {
            using (BadBeeEntities db = new BadBeeEntities())
            {
                return db.Item.Count();
            }
        }
    }
}