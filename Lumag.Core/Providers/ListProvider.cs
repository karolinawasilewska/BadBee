using LinqKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Lumag.Core.Models;
using Lumag.Core.DAL;

namespace Lumag.Core.Providers
{
    public class ListProvider : IDisposable
    {
        private LumagEntities db;
        private bool IsDisposed;
        private BadBeeDBEntities BadBeeDBEntities;

        public ListProvider()
        {
            db = new LumagEntities();
            BadBeeDBEntities = new BadBeeDBEntities();
        }

        public class ListResult
        {
            public List<CvlItem> Items { get; set; }
            public int ItemsCount { get; set; }
        }

        public ListResult GetList(LumagFilter filter, int page, int itemsPerPage)

        {
            ListResult results = new ListResult();
            results.Items = new List<CvlItem>();
            results.ItemsCount = 0;
            var query = db.ItemsDb.AsQueryable();

            if (filter.PhraseFilter != null)
            {
                //if (GetChList<Brands>().Any(row => row.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())))
                //{
                //    List<Brands> br = GetChList<Brands>().Where(q => q.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())).ToList();
                //    List<string> brandIds = br.Select(x => x.Id.ToString()).ToList();
                //    query = query.Where(q => brandIds.Contains(q.BrandId.ToString()));
                //    filter.BrandsList = brandIds;
                //}
                //else if (GetChList<Series>().Any(row => row.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())))
                //{
                //    List<Series> ser = GetChList<Series>().Where(q => q.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())).ToList();
                //    List<string> serieIds = ser.Select(x => x.Id.ToString()).ToList();
                //    query = query.Where(q => serieIds.Contains(q.SerieId.ToString()));
                //    filter.SeriesList = serieIds;
                //}
                //else if (GetChList<DAL.Models>().Any(row => row.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())))
                //{
                //    List<DAL.Models> mod = GetChList<DAL.Models>().Where(q => q.Name.ToLower().StartsWith(filter.PhraseFilter.ToLower())).ToList();
                //    List<string> modelIds = mod.Select(x => x.Id.ToString()).ToList();
                //    query = query.Where(q => modelIds.Contains(q.ModelId.ToString()));
                //    filter.ModelsList = modelIds;
                //}
                //else
                 if (GetChList<DAL.LumagNumbers>().Any(q => 
                (q.LumagNumber.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                .StartsWith(filter.PhraseFilter.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")))))
                {
                    List<LumagNumbers> lum = GetChList<LumagNumbers>().Where(q => (q.LumagNumber.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                        .StartsWith(filter.PhraseFilter.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")))).ToList();

                    List<string> lumagIds = lum.Select(x => x.LumagNumberId.ToString()).ToList();
                    query = query.Where(q => lumagIds.Contains(q.LumagNumberId.ToString()));
                    if (lum != null)
                    {
                        filter.LumagNumbersList = lumagIds;
                    }
                }
                else if (GetChList<Wva>().Any(q => q.Name.StartsWith(filter.PhraseFilter.Replace(" ", ""))))
                {
                    List<Wva> wva = GetChList<Wva>().Where(q => q.Name.Replace(" ", "").StartsWith(filter.PhraseFilter.Replace(" ", ""))).ToList();

                    List<string> wvaIds = wva.Select(x => x.Id.ToString()).ToList();
                    query = query.Where(q => wvaIds.Contains(q.WvaId.ToString()));
                    if (wva != null)
                    {
                        filter.WvasList = wvaIds;
                    }
                }
                else if (GetChList<WvaDetails>().Any(q => q.Name.StartsWith(filter.PhraseFilter.Replace(" ",""))))
                {

                    List<WvaDetails> wvadetails = GetChList<WvaDetails>().Where(q => q.Name.Replace(" ", "").StartsWith(filter.PhraseFilter.Replace(" ", ""))).ToList();
                    List<string> wvaIds = wvadetails.Select(x => x.WvaId.ToString()).ToList();
                   
                    query = query.Where(q => wvaIds.Contains(q.WvaId.ToString()));
                    if (wvadetails != null)
                    {
                        filter.WvasList = wvaIds;
                    }
                }
                else {
                    List<Crosses> cross = new List<Crosses>();

                    cross = GetChList<Crosses>().Where(q => (q.CrossBrandNumber.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                        .StartsWith(filter.PhraseFilter.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")))).ToList();
                    
                    if (cross.Count()==0 && filter.PhraseFilter.StartsWith("0"))
                    {
                        cross= GetChList<Crosses>().Where(q => (q.CrossBrandNumber.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                        .StartsWith(filter.PhraseFilter.Remove(0, 1).Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")))).ToList();
                    }
                    else if(cross.Count() == 0 && GetChList<Crosses>().Any(row => row.CrossBrandNumber.ToLower().Replace(" ","").StartsWith("0"+filter.PhraseFilter.Replace(" ", "").ToLower())))
                    {
                        cross= GetChList<Crosses>().Where(q=>(q.CrossBrandNumber).Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
                        .StartsWith("0" + filter.PhraseFilter.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", ""))).ToList();
                    }

                    List <string> crossIds = cross.Select(x => x.LumagNumberId.ToString()).ToList();
                    if (cross!=null)
                    {
                        filter.LumagNumbersList = crossIds;
                        query = query.Where(q => crossIds.Contains(q.LumagNumberId.ToString()));
                    }
                    
                    else
                {
                        return results;
                    }
                }
                

                filter.PhraseFilter = null;
            }

            if (filter != null && filter.PhraseFilter==null)
            {
                if (!string.IsNullOrEmpty(filter.CrossNumbers))
                {
                    query = query.Where(q => filter.CrossList.Contains(q.LumagNumberId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Models))
                {
                    query = query.Where(q => filter.ModelsList.Contains(q.ModelId.ToString()));
                }

                else if (!string.IsNullOrEmpty(filter.Series))
                {
                    query = query.Where(q => filter.SeriesList.Contains(q.SerieId.ToString()));
                }
                else if (!string.IsNullOrEmpty(filter.Brands))
                {
                    query = query.Where(q => filter.BrandsList.Contains(q.BrandId.ToString()));
                }
                if (string.IsNullOrEmpty(filter.Brands))
                {
                    filter.Series = null;
                    filter.Models = null;
                }
                if (!string.IsNullOrEmpty(filter.LumagNumbers))
                {
                    query = query.Where(q => filter.LumagNumbersList.Contains(q.LumagNumberId.ToString()));
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

                    query = query.Where(q => yslist.Contains(q.DateFrom.Value.Year) || yslist.Contains(q.DateTo.Value.Year));
                }
                if (!string.IsNullOrEmpty(filter.Wvas))
                {
                    query = query.Where(q => filter.WvasList.Contains(q.WvaId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.WvaDetails2))
                {
                    query = query.Where(q => filter.WvasDetailsList.Contains(q.WvaDetailsId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Widths))
                {
                    query = query.Where(q => filter.WidthsList.Contains(q.WidthId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Heights))
                {
                    query = query.Where(q => filter.HeightsList.Contains(q.HeightId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Thicknesses))
                {
                    query = query.Where(q => filter.ThicknessesList.Contains(q.ThicknessId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.DrumDiameters))
                {
                    query = query.Where(q => filter.DrumDiametersList.Contains(q.DrumDiameterId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Systems))
                {
                    query = query.Where(q => filter.SystemsList.Contains(q.SystemId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Rivets))
                {
                    query = query.Where(q => filter.RivetsList.Contains(q.RivetTypeId.ToString()));
                }
            }
            query = query.OrderBy(q => q.Brand).ThenBy(q => q.Serie).ThenBy(q=>q.Model).ThenBy(q=>q.Id);
            results.ItemsCount = query.Count();
            query = query.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
            var list = query.ToList();
            filter.PhraseFilter = null;
            foreach (ItemsDb items in list)
            {
                string sYears = "";

                if (items.DateFrom.HasValue)
                {
                    sYears += items.DateFrom.Value.ToString("MM/yy", CultureInfo.InvariantCulture);

                    if (items.DateTo.HasValue)
                    {
                        sYears += " - " + items.DateTo.Value.ToString("MM/yy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        sYears += " ->";
                    }
                }
                else
                {
                    if (items.DateTo.HasValue)
                    {
                        sYears += "<- " + items.DateTo.Value.ToString("MM/yy", CultureInfo.InvariantCulture);
                    }
                }


                CvlItem newItem = new CvlItem()
                {
                    Id=items.Id,
                    LumagNumber = items.LumagNumber.TrimEnd(),
                    Brand = items.Brand.TrimEnd(),
                    Serie = items.Serie.TrimEnd(),
                    Model = items.Model.TrimEnd(),
                    Kw = items.Kw.TrimEnd(),
                    Km = items.Km.TrimEnd(),
                    Years = sYears.TrimEnd(),
                    Fr = items.Fr.TrimEnd(),
                    WvaDesc = items.WvaDesc.TrimEnd(),
                    Wva = items.Wva.TrimEnd(),
                    WvaDetailsQty = items.WvaDetailsQty.TrimEnd(),
                    WvaDetails = items.WvaDetails.TrimEnd(),
                    Width = items.Width.TrimEnd(),
                    Height = items.Height.TrimEnd(),
                    Thickness = items.Thickness.TrimEnd(),
                    Wedge = items.Wedge.TrimEnd(),
                    DrumDiameter = items.DrumDiameter.TrimEnd(),
                    BrakeSystem = items.BrakeSystem.TrimEnd(),
                    RivetsQuantity = items.RivetsQuantity.TrimEnd(),
                    RivetsType = items.RivetsType.TrimEnd(),
                    Product=items.ProductType.TrimEnd()
                };

                results.Items.Add(newItem);

            }

            return results;
        }

        public static void FillDictionaryCache()
        {
            using (LumagEntities db = new LumagEntities())
            {
                GlobalVars.DictionaryCache.Add(typeof(Brands), db.Brands.OrderBy(q=>q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Series), db.Series.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(DAL.Models), db.Models.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(LumagNumbers), db.LumagNumbers.OrderBy(q => q.LumagNumber).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Wva), db.Wva.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(WvaDetails), db.WvaDetails.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(DrumDiameters), db.DrumDiameters.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Systems), db.Systems.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(RivetTypes), db.RivetTypes.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Widths), db.Widths.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Thicknesses), db.Thicknesses.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Years), db.Years.OrderBy(q => q.Name).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Crosses), db.Crosses.OrderBy(q => q.CrossBrandNumber).ToList());
                GlobalVars.DictionaryCache.Add(typeof(Heights), db.Heights.OrderBy(q => q.Name).ToList());
            }
        }

        public List<T> GetChList<T>()
        {
            return GlobalVars.DictionaryCache[typeof(T)] as List<T>;
        }
        public List<Brands> GetBrandsChList(LumagFilter filter)
        {            
            return GetChList<Brands>().Where(q => filter.BrandsList.Contains(q.Id.ToString())).OrderBy(q=>q.Name).ToList();
        }
        public List<Series> GetSeriesChList(LumagFilter filter)
        {
            return GetChList<Series>().Where(q => filter.SeriesList.Contains(q.Id.ToString())).ToList();
        }
        public List<DAL.Models> GetModelsChList(LumagFilter filter)
        {
            return GetChList<DAL.Models>().Where(q => filter.ModelsList.Contains(q.Id.ToString())).ToList();
        }
        public List<LumagNumbers> GetLumagChList(LumagFilter filter)
        {
            if (filter.CrossList == null)
            {
                return GetChList<LumagNumbers>().Where(q => (filter.LumagNumbersList.Contains(q.LumagNumberId.ToString()))).ToList();
            }
            else
            {
                return GetChList<LumagNumbers>().Where(q => (filter.LumagNumbersList.Contains(q.LumagNumberId.ToString())) || (filter.CrossList.Contains(q.LumagNumberId.ToString()))).ToList();
            }
        }
        public List<Wva> GetWvaChList(LumagFilter filter)
        {
            return GetChList<Wva>().Where(q => filter.WvasList.Contains(q.Id.ToString())).ToList();
        }
        public List<WvaDetails> GetWvaDetailsChList(LumagFilter filter)
        {
            return GetChList<WvaDetails>().Where(q => filter.WvasDetailsList.Contains(q.Id.ToString())).ToList();
        }
        public List<DrumDiameters> GetDrumDiametersListCh(LumagFilter filter)
        {
            return GetChList<DrumDiameters>().Where(q => filter.DrumDiametersList.Contains(q.Id.ToString())).ToList();
        }
        public List<Systems> GetSystemsListCh(LumagFilter filter)
        {
            return GetChList<Systems>().Where(q => filter.SystemsList.Contains(q.Id.ToString())).ToList();
        }
        public List<RivetTypes> GetRivetListCh(LumagFilter filter)
        {
            return GetChList<RivetTypes>().Where(q => filter.RivetsList.Contains(q.Id.ToString())).ToList();
        }
        public List<Widths> GetWidthsListCh(LumagFilter filter)
        {
            return GetChList<Widths>().Where(q => filter.WidthsList.Contains(q.Id.ToString())).ToList();
        }
        public List<Thicknesses> GetThicknessesListCh(LumagFilter filter)
        {
            return GetChList<Thicknesses>().Where(q => filter.ThicknessesList.Contains(q.Id.ToString())).ToList();
        }
        public List<Heights> GetHeightsListCh(LumagFilter filter)
        {
            return GetChList<Heights>().Where(q => filter.HeightsList.Contains(q.Id.ToString())).ToList();
        }
        public List<Years> GetYearsListCh(LumagFilter filter)
        {
            return GetChList<Years>().Where(q => filter.DateYearsList.Contains(q.Id.ToString())).ToList();
        }
        public List<Crosses> GetCrossesListCh(LumagFilter filter)
        {
            return GetChList<Crosses>().Where(q => filter.CrossList.Contains(q.LumagNumberId.ToString())).ToList();
        }

        public List<ItemsDb> GetList(LumagFilter filter, bool brand, bool serie, bool model, bool drum, bool height, bool width, 
            bool thick, bool system, bool rivet, bool wva, bool wvad, bool lum, bool year)
        {
            if (string.IsNullOrEmpty(GlobalVars.SearchCache.SearchKey) == false && GlobalVars.SearchCache.SearchKey.Equals(filter.SearchKey))
            {
                Debug.WriteLine(DateTime.Now + " Cache search with key " + GlobalVars.SearchCache.SearchKey);

                return GlobalVars.SearchCache.GetListResult;
            }
            else
            {
                var items = db.ItemsDb.AsQueryable();

                if (!string.IsNullOrEmpty(filter.CrossNumbers))
                {
                    items = items.Where(q => filter.CrossList.Contains(q.LumagNumberId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Models) && !model)
                {
                    items = items.Where(q => filter.ModelsList.Contains(q.ModelId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Series) && !serie)
                {
                    items = items.Where(q => filter.SeriesList.Contains(q.SerieId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Brands) && !brand)
                {
                    items = items.Where(q => filter.BrandsList.Contains(q.BrandId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.DrumDiameters) && !drum)
                {
                    items = items.Where(q => filter.DrumDiametersList.Contains(q.DrumDiameterId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Heights) && !height)
                {
                    items = items.Where(q => filter.HeightsList.Contains(q.HeightId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Widths) && !width)
                {
                    items = items.Where(q => filter.WidthsList.Contains(q.WidthId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Thicknesses) && !thick)
                {
                    items = items.Where(q => filter.ThicknessesList.Contains(q.ThicknessId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Systems) && !system)
                {
                    items = items.Where(q => filter.SystemsList.Contains(q.SystemId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Rivets) && !rivet)
                {
                    items = items.Where(q => filter.RivetsList.Contains(q.RivetTypeId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.Wvas) && !wva)
                {
                    items = items.Where(q => filter.WvasList.Contains(q.WvaId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.WvaDetails2) && !wvad)
                {
                    items = items.Where(q => filter.WvasDetailsList.Contains(q.WvaDetailsId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.LumagNumbers) && !lum)
                {
                    items = items.Where(q => filter.LumagNumbersList.Contains(q.LumagNumberId.ToString()));
                }
                if (!string.IsNullOrEmpty(filter.DateYears) && !year)
                {
                    items = items.Where(q => filter.DateYearsList.Contains(q.DateFrom.Value.Year.ToString()) || filter.DateYearsList.Contains(q.DateTo.Value.Year.ToString()));
                }

                var searchCache = new SearchCache();
                searchCache.GetListResult = items.ToList();
                searchCache.SearchKey = filter.SearchKey;

                GlobalVars.SearchCache = searchCache;

                Debug.WriteLine(DateTime.Now + " New search with key " + GlobalVars.SearchCache.SearchKey);

                return GlobalVars.SearchCache.GetListResult;
            }
        }
        public List<Crosses> GetCrossesList(LumagFilter filter)
        {
            var items = GetChList<Crosses>();
           // var distinctRowsById = lum.Select(i => i.LumagNumberId).Distinct().Select(i => lum.First(o => o.LumagNumberId == i)).ToList();
            // return GetChList<Crosses>().Where(q => q.CrossBrandName.ToLower() == filter.CrossName.ToLower()).OrderBy(q => q.CrossBrandNumber).ToList();
            return items.Select(q=>q.CrossBrandNumber).Distinct().Select(q=> items.First(o=>o.CrossBrandNumber==q)).OrderBy(q=>q.CrossBrandNumber).ToList();
        }

        public List<Brands> GetBrandsList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && /*string.IsNullOrEmpty(filter.Brands) && */string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Brands>();
            }
            else
            {
                var items = GetList(filter, true, false, false,false,false, false, false, false, false, false, false, false, false);
                var list = items.OrderBy(q => q.Brand).ToList();
                var lum = list.Select(item => new Brands { Id = item.BrandId ?? default(int), Name = item.Brand.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }

        }
        public List<LumagNumbers> GetLumagList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) /*&& string.IsNullOrEmpty(filter.LumagNumbers)*/ && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<LumagNumbers>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, false, true, false);
                var list = items.OrderBy(q => q.LumagNumber).ToList();
                var lum = list.Select(item => new LumagNumbers { LumagNumber = item.LumagNumber.TrimEnd(), LumagNumberId = item.LumagNumberId ?? default(int) }).ToList();
                var distinctRowsById = lum.Select(i => i.LumagNumberId).Distinct().Select(i => lum.First(o => o.LumagNumberId == i)).ToList();

                return distinctRowsById.OrderBy(q => q.LumagNumber).ToList();
            }
        }
        public List<string> GetPadsWvasList()
        {
            var items = db.ItemsDb.Where(q => q.ProductType == "Brake pad").ToList();
            List<Wva> wva = items.Distinct().Where(q => q.WvaId != 0).Select(item => new Wva { Id = item.WvaId ?? default(int), Name = item.Wva }).ToList();
            var distinctWvasRowsById = wva.Select(i => i.Id).Distinct().Select(i => wva.First(o => o.Id == i)).ToList();
            List<string> wvaNames = distinctWvasRowsById.Select(s => s.Name).ToList();
            return wvaNames.OrderBy(q=>q).ToList();
        }
        public List<string> GetPadsLumagsList()
        {
            var items = db.ItemsDb.Where(q => q.ProductType == "Brake pad").ToList();
            List<LumagNumbers> lumag = items.Distinct().Where(q => q.LumagNumberId != 0).Select(item => new LumagNumbers { LumagNumberId = item.LumagNumberId ?? default(int), LumagNumber = item.LumagNumber }).ToList();
            var distinctLumagsRowsById = lumag.Select(i => i.LumagNumberId).Distinct().Select(i => lumag.First(o => o.LumagNumberId == i)).ToList();
            List<string> lumagNames = distinctLumagsRowsById.Select(s => s.LumagNumber).ToList();
            return lumagNames.OrderBy(q => q).ToList(); ;
        }
        public List<Wva> GetWvaList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) /*&& string.IsNullOrEmpty(filter.Wvas)*/
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Wva>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, false, true, false, false, false);

                var list = items.OrderBy(q => q.Wva).ToList();
                var lum = list.Distinct().Select(item => new Wva { Id = item.WvaId ?? default(int), Name = item.Wva.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        
        public List<WvaDetails> GetWvaDetList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                   /* && string.IsNullOrEmpty(filter.WvaDetails2)*/ && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<WvaDetails>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, true, false, false);
                var list = items.OrderBy(q => q.WvaDetails).ToList();
                var lum = list.Select(item => new WvaDetails { Id = item.WvaDetailsId ?? default(int), Name = item.WvaDetails.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<Years> GetYearsList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    /*&& string.IsNullOrEmpty(filter.DateYears)*/ && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Years>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, false, false, false, false, true);
                var list = items.ToList();
                var lum1 = list.Distinct().Select(item => new Years { Id = item.DateFrom.HasValue ? item.DateFrom.Value.Year : 0 , Name = item.DateFrom.HasValue ? item.DateFrom.Value.Year.ToString() : string.Empty }).ToList();
                var lum2 = list.Distinct().Select(item => new Years { Id = item.DateTo.HasValue ? item.DateTo.Value.Year : 0 , Name = item.DateTo.HasValue ? item.DateTo.Value.Year.ToString() : string.Empty }).ToList();
                var lum= lum1.Concat(lum2).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<Heights> GetHeightsList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths)/* && string.IsNullOrEmpty(filter.Heights) */&& string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<Heights>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, true, false, false, false, false, false, false, false, false);
                var list = items.ToList();
                var lum = list.Select(item => new Heights { Id = item.HeightId ?? default(int), Name = item.Height.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<Widths> GetWidthsList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    /*&& string.IsNullOrEmpty(filter.Widths)*/ && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {                
                return GetChList<Widths>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, true, false, false, false, false, false, false, false);
                var list = items.ToList();
                var lum = list.Select(item => new Widths { Id = item.WidthId ?? default(int), Name = item.Width.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<Thicknesses> GetThicknessesList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) /*&& string.IsNullOrEmpty(filter.Thicknesses)*/
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {                
                return GetChList<Thicknesses>();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, true, false, false, false, false, false, false);
                var list = items.ToList();
                var lum = list.Select(item => new Thicknesses { Id = item.ThicknessId ?? default(int), Name = item.Thickness.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<DrumDiameters> GetDrumDiametersList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) /*&& string.IsNullOrEmpty(filter.DrumDiameters)*/ && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {                
                return GetChList<DrumDiameters>();
            }
            else
            {
                var items = GetList(filter, false, false, false, true, false, false, false, false, false, false, false, false, false);
                var list = items.ToList();
                var lum = list.Select(item => new DrumDiameters { Id = item.DrumDiameterId ?? default(int), Name = item.DrumDiameter.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<Systems> GetSystemsList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
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
                var lum = list.Select(item => new Systems { Id = item.SystemId ?? default(int), Name = item.BrakeSystem.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<RivetTypes> GetRivetTypesList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series) && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) /*&& string.IsNullOrEmpty(filter.Rivets)*/
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems)/* && string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {                
                return GetChList<RivetTypes>().OrderBy(q=>q.Id).ToList();
            }
            else
            {
                var items = GetList(filter, false, false, false, false, false, false, false, false, true, false, false, false, false);
                var list = items.ToList();
                var lum = list.Select(item => new RivetTypes { Id =item.RivetTypeId ?? default(int), Name = item.RivetsType.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q => q.Name).ToList();
            }
        }
        public List<Series> GetSeriesList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) /*&& string.IsNullOrEmpty(filter.Series)*/ && string.IsNullOrEmpty(filter.Models)
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {                
                return GetChList<Series>().OrderBy(q=>q.Name).ToList();
            }
            else
            {
                var items = GetList(filter, false, true, false, false, false, false, false, false, false, false, false, false, false);
                var list = items.ToList();
                var lum = list.Select(item => new Series { Id = item.SerieId ?? default(int), Name = item.Serie.TrimEnd() }).ToList();
                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();

                return distinctRowsById.OrderBy(q=>q.Name).ToList();
            }
        }
        public List<DAL.Models> GetModelsList(LumagFilter filter)
        {            
            if (string.IsNullOrEmpty(filter.CrossNumbers) && string.IsNullOrEmpty(filter.Brands) && string.IsNullOrEmpty(filter.Series)/* && string.IsNullOrEmpty(filter.Models)*/
                    && string.IsNullOrEmpty(filter.DateYears) && string.IsNullOrEmpty(filter.LumagNumbers) && string.IsNullOrEmpty(filter.Wvas)
                    && string.IsNullOrEmpty(filter.WvaDetails2) && string.IsNullOrEmpty(filter.DrumDiameters) && string.IsNullOrEmpty(filter.Rivets)
                    && string.IsNullOrEmpty(filter.Widths) && string.IsNullOrEmpty(filter.Heights) && string.IsNullOrEmpty(filter.Thicknesses)
                    && string.IsNullOrEmpty(filter.Systems) /*&& string.IsNullOrEmpty(filter.PhraseFilter)*/)
            {
                return GetChList<DAL.Models>();
            }
            else
            {
                var items = GetList(filter, false, false, true, false, false, false, false, false, false, false, false, false, false);
                var list = items.ToList();
                var lum = list.Select(item => new DAL.Models { Id = item.ModelId ?? default(int), Name = item.Model.TrimEnd() }).ToList();

                var distinctRowsById = lum.Select(i => i.Id).Distinct().Select(i => lum.First(o => o.Id == i)).ToList();
               
                return distinctRowsById;
            }
        }

        public List<string> GetKeywords(string keywordPart)
        {
            return db.GetKeywords(keywordPart).OrderBy(q=>q).Take(5).ToList();
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
            using (LumagEntities db = new LumagEntities())
            {
                return db.ItemsDb.Count();
            }
        }
    }
}