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
                    List<DAL.BadBee> bb = GetChList<DAL.BadBee>().Where(q => (q.BadBeeNo.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                        .StartsWith(filter.PhraseFilter.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")))).ToList();

                    List<string> badBeeIds = bb.Select(x => x.BadBeeId.ToString()).ToList();
                    query = query.Where(q => badBeeIds.Contains(q.BadBeeId.ToString()));
                    if (bb != null)
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

                    query = query.Where(q => yslist.Contains(int.Parse(q.Model.Year.DateFromFK.Date1)) || yslist.Contains(int.Parse(q.Model.Year.DateToFK.Date1)));
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

                    if (item.Model.Year!=null && item.Model.Year.DateFromId!=0)
                    {
                        sYears += item.Model.Year.DateToFK.Date1;

                        if (item.Model.Year.DateToId!=0)
                        {
                            sYears += " - " + item.Model.Year.DateToFK.Date1;
                        }
                        else
                        {
                            sYears += " ->";
                        }
                    }
                    else
                    {
                        if (item.Model.Year!=null  && item.Model.Year.DateToId!=0)
                        {
                            sYears += "<- " + item.Model.Year.DateToFK.Date1;
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
                GlobalVars.DictionaryCache.Add(typeof(Date), db.Date.OrderBy(q => q.Date1).ToList());
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
        public List<Date> GetYearsListCh(BadBeeFilter filter)
        {
            return GetChList<Date>().Where(q => filter.DateYearsList.Contains(q.DateId.ToString())).ToList();
        }
       

        public List<Item> GetList(BadBeeFilter filter, bool brand, bool serie, bool model, bool drum, bool height, bool width, 
            bool thick, bool system, bool rivet, bool wva, bool wvad, bool bb, bool year)
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
                if (!string.IsNullOrEmpty(filter.BadBeeNumbers) && !bb)
                {
                    items = items.Where(q => filter.BadBeeNumbersList.Contains(q.BadBeeId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.DateYears) && !year)
                {
                    items = items.Where(q => filter.DateYearsList.Contains(q.Model.Year.DateFromFK.Date1) || filter.DateYearsList.Contains(q.Model.Year.DateToFK.Date1));
                }

                var searchCache = new SearchCache();
                searchCache.GetListResult = items.ToList();
                searchCache.SearchKey = filter.SearchKey;

                GlobalVars.SearchCache = searchCache;

                Debug.WriteLine(DateTime.Now + " New search with key " + GlobalVars.SearchCache.SearchKey);

                return GlobalVars.SearchCache.GetListResult;
            }
        }
        public List<Brand> GetBrandsList(BadBeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && /*string.IsNullOrEmpty(filter.Brands) && */string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Brand>();
            }

            else
            {
                var items = GetList(filter, true, false, false, false, false, false, false, false, false, false, false, false, false);
                var list = items.ToList();
                var bb = list.Select(item => new Brand { BrandId = item.Model.Serie.Brand.BrandId, Name = item.Model.Serie.Brand.Name.TrimEnd() }).ToList();
                var distinctRowsById = bb.Select(i => i.Name).Distinct().Select(i => bb.First(o => o.Name == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }

        }
        public List<DAL.BadBee> GetBadBeeList(BadBeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) /*&& string.IsNullOrEmpty(filter.BadBeeNumbers)*/ && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<DAL.BadBee>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, false, true, false);
                var list = items.OrderBy(q => q.BadBee.BadBeeNo).ToList();
                var bb = list.Select(item => new BadBee.Core.DAL.BadBee { BadBeeNo = item.BadBee.BadBeeNo.TrimEnd(), BadBeeId = item.BadBeeId }).ToList();
                var distinctRowsById = bb.Select(i => i.BadBeeId).Distinct().Select(i => bb.First(o => o.BadBeeId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.BadBeeNo).ToList();
            }
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
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) /*&& string.IsNullOrEmpty(filter.Wvas)*/
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Wva>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, false, true, false, false, false);

                var list = items.OrderBy(q => q.BadBee.Wva.WvaNo).ToList();
                var bb = list.Distinct().Select(item => new Wva { WvaId = item.BadBee.Wva.WvaId, WvaNo = item.BadBee.Wva.WvaNo.TrimEnd() }).ToList();
                var distinctRowsById = bb.Select(i => i.WvaId).Distinct().Select(i => bb.First(o => o.WvaId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.WvaNo).ToList();
            }
        }
        public List<Date> GetYearsList(BadBeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    /*&& string.IsNullOrEmpty(filter.DateYears)*/ && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Date>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, false, false, true);
                var list = items.ToList();
                var bb = list.Distinct().Select(item => new Date { Date1 = item.Model.Year.DateFromFK.Date1 ?? "", DateId = item.Model.Year.DateFromFK.DateId }).ToList();
                var distinctRowsById = bb.Select(i => i.DateId).Distinct().Select(i => bb.First(o => o.DateId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Date1).ToList();
            }
        }
        public List<Height> GetHeightsList(BadBeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths)/* && string.IsNullOrEmpty(filter.Heights) */&& string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Height>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, true, false, false, false, false, false, false, false, false);
                var list = items.ToList();
                var bb = list.Select(item => new Height { HeightId = item.BadBee.Dimension.Height.HeightId, Height1 = item.BadBee.Dimension.Height.Height1}).ToList();
                var distinctRowsById = bb.Select(i => i.HeightId).Distinct().Select(i => bb.First(o => o.HeightId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Height1).ToList();
            }
        }
        public List<Width> GetWidthsList(BadBeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    /*&& string.IsNullOrEmpty(filter.Widths)*/ && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Width>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, true, false, false, false, false, false, false, false);
                var list = items.ToList();
                var bb = list.Select(item => new Width { WidthId = item.BadBee.Dimension.Width.WidthId, Width1 = item.BadBee.Dimension.Width.Width1 }).ToList();
                var distinctRowsById = bb.Select(i => i.WidthId).Distinct().Select(i => bb.First(o => o.WidthId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Width1).ToList();
            }
        }
        public List<Thickness> GetThicknessesList(BadBeeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) /*&& string.IsNullOrEmpty(filter.Thicknesses)*/
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Thickness>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, true, false, false, false, false, false, false);
                var list = items.ToList();
                var bb = list.Select(item => new Thickness { ThicknessId = item.BadBee.Dimension.Thickness.ThicknessId, Thickness1 =item.BadBee.Dimension.Thickness.Thickness1 }).ToList();
                var distinctRowsById = bb.Select(i => i.ThicknessId).Distinct().Select(i => bb.First(o => o.ThicknessId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Thickness1).ToList();
            }
        }
        public List<Systems> GetSystemsList(BadBeeFilter filter)
            {
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    /*&& string.IsNullOrEmpty(filter.Systems)*/ /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Systems>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, true, false, false, false, false, false);
                var list = items.ToList();
                var bb = list.Select(item => new Systems { SystemId = item.BadBee.Systems.SystemId, Abbreviation = item.BadBee.Systems.Abbreviation.TrimEnd() }).ToList();
                var distinctRowsById = bb.Select(i => i.SystemId).Distinct().Select(i => bb.First(o => o.SystemId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Abbreviation).ToList();
            }
        }
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
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series)/* && string.IsNullOrEmpty(filter.Models)*/
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.BadBeeNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Model>();
            }
            else
            {
                var items = GetList(filter, false, false, true, false, false, false, false, false, false, false, false, false, false);
                var list = items.ToList();
                var bb = list.Select(item => new Model {ModelId = item.ModelId, Name = item.Model.Name.TrimEnd() }).ToList();

                var distinctRowsById = bb.Select(i => i.ModelId).Distinct().Select(i => bb.First(o => o.ModelId == i)).ToList();

                return distinctRowsById;
            }
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