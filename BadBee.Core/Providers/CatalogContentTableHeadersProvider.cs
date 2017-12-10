using BadBee.Core.DAL;
using BadBee.Core.Models;
using BadBee.Core.MyResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadBee.Core.Providers
{
    public class CatalogContentTableHeadersProvider
    {
        //public List<CatalogContentTableHeader> CreateTopHeader()
        //{
        //    List<CatalogContentTableHeader> items = new List<CatalogContentTableHeader>();

        //    items.Add(new CatalogContentTableHeader()
        //    {
        //        Type = CatalogContentTableHeaderType.TopForCars,
        //        //Colspan = "4"
        //    });
        //    items.Add(new CatalogContentTableHeader()
        //    {
        //        Type = CatalogContentTableHeaderType.TopForCars,
        //        PictureSrc = "../../Images/head_brand_serie_model_kw.png",
        //        Colspan = "5"
        //    });
        //    items.Add(new CatalogContentTableHeader()
        //    {
        //        Type = CatalogContentTableHeaderType.TopForCars,
        //        Colspan = "14"
        //    });
        //    return items;
        //}
        public List<CatalogContentTableHeader> CreateTopHeaderItems()
        {
            List<CatalogContentTableHeader> items = new List<CatalogContentTableHeader>();

            

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#brandFilterModal",
                Description = @Resources.brand,
                ID = "brand",
                OnClickMethod = "showBrands()",
              //  PictureSrc = "../../Images/nothing_head.png",
                IsFilterSelected = GlobalVars.BadBeeFilter.BrandsSelected,
                CssStyle = "cursor: pointer; border-right:0 !important;",
                HasAnyElements = HasBrandsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {                
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#serieFilterModal",
                Description = @Resources.serie,
                ID = "seriesmod",
                OnClickMethod = "showSeries()",
             //   PictureSrc = "../../Images/nothing_head.png",
                IsFilterSelected = GlobalVars.BadBeeFilter.SeriesSelected,
                CssStyle= "border-right:0 !important;",
                HasAnyElements = HasSeriesFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#modelFilterModal",
                Description = @Resources.model,
                ID = "modelsmod",
                OnClickMethod = "showModels()",
               // PictureSrc = "../../Images/nothing_head.png",
                IsFilterSelected = GlobalVars.BadBeeFilter.ModelsSelected,
                CssStyle = "border-right:0 !important;",
                HasAnyElements = HasModelsFilterAnyElements()
            });

           
            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#yearsFilterModal",
                Description = @Resources.year,
                ID = "years",
                OnClickMethod = "showYears()",
             //   PictureSrc = "../../Images/year_head_tran.png",
                IsFilterSelected = GlobalVars.BadBeeFilter.DateYearsSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasYearsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "fr",
                Description = @Resources.front_rear,
              //  PictureSrc = "../../Images/fr_head_tran.png"
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "wva",
                Description = @Resources.wva,
            //    PictureSrc = "../../Images/wva_head_tran.png",
                Colspan = "2"
            });
            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#BadBeeFilterModal",
                Description = @Resources.BadBee_no,
                ID = "BadBeenumber",
                OnClickMethod = "showBadBee()",
            //    PictureSrc = "../../Images/BadBee_head_tran.png",
                IsFilterSelected = GlobalVars.BadBeeFilter.BadBeeNumbersSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasBadBeesFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "size",
                Description = @Resources.size,
            //    PictureSrc = "../../Images/size_head_tran.png",
                Colspan = "3"
            });
            
         
            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#systemFilterModal",
                Description = @Resources.system,
                ID = "sys",
                OnClickMethod = "showSystems()",
            //    PictureSrc = "../../Images/brake_head_tran.png",
                IsFilterSelected = GlobalVars.BadBeeFilter.SystemsSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasSystemsFilterAnyElements()
            });

            return items;
        }
        public List<CatalogContentTableHeader> CreateDetailsHeaderItems()
        {
            List<CatalogContentTableHeader> items = new List<CatalogContentTableHeader>();

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Colspan = "3"
            });

          

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Description = Resources.description
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#wvaFilterModal",
                Description = @Resources.wva,
                ID = "wva",
                OnClickMethod = "showWva()",
                IsFilterSelected = GlobalVars.BadBeeFilter.WvasSelected,
                CssStyle = "cursor: pointer;",
             //   HasAnyElements = HasWvaFilterAnyElements()
            });


            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#heightFilterModal",
                Description = @Resources.h,
                ID = "height",
                OnClickMethod = "showHeights()",
                IsFilterSelected = GlobalVars.BadBeeFilter.HeightsSelected,
                CssStyle = "cursor: pointer;",
             //   HasAnyElements = HasHeightsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#widthFilterModal",
                Description = @Resources.w,
                ID = "width",
                OnClickMethod = "showWidths()",
                IsFilterSelected = GlobalVars.BadBeeFilter.WidthsSelected,
                CssStyle = "cursor: pointer;",
             //   HasAnyElements = HasWidthsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#thicknessFilterModal",
                Description = @Resources.t,
                ID = "thickness",
                OnClickMethod = "showThicknesses()",
                IsFilterSelected = GlobalVars.BadBeeFilter.ThicknessesSelected,
                CssStyle = "cursor: pointer;",
             //   HasAnyElements = HasThicknessesFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Colspan = "3"
            });

            return items;
        }

        public bool HasYearsFilterAnyElements()
        {
            List<Year> year = new List<Year>();
            using (ListProvider provider = new ListProvider())
            {
                year = provider.GetYearsListCh(GlobalVars.BadBeeFilter);
                year.AddRange(provider.GetYearsList(GlobalVars.BadBeeFilter));
            }
            return year.Where(q => q.YearId != 0).Count() > 0;
        }

        public bool HasHeightsFilterAnyElements()
        {
            List<Dimension> hei = new List<Dimension>();
            using (ListProvider provider = new ListProvider())
            {
                hei = provider.GetWidthsListCh(GlobalVars.BadBeeFilter);
                hei.AddRange(provider.GetHeightsList(GlobalVars.BadBeeFilter));
            }
            return hei.Where(q => q.DimensionId != 0).Count() > 0;
        }

        public bool HasWidthsFilterAnyElements()
        {
            List<Dimension> wid = new List<Dimension>();
            using (ListProvider provider = new ListProvider())
            {
                wid = provider.GetWidthsListCh(GlobalVars.BadBeeFilter);
                wid.AddRange(provider.GetWidthsList(GlobalVars.BadBeeFilter));
            }
            return wid.Where(q => q.DimensionId != 0).Count() > 0;
        }

        public bool HasThicknessesFilterAnyElements()
        {
            List<Dimension> thick = new List<Dimension>();
            using (ListProvider provider = new ListProvider())
            {
                thick = provider.GetWidthsListCh(GlobalVars.BadBeeFilter);
                thick.AddRange(provider.GetThicknessesList(GlobalVars.BadBeeFilter));
            }
            return thick.Where(q => q.DimensionId != 0).Count() > 0;
        }

        public bool HasSystemsFilterAnyElements()
        {
            List<Systems> system = new List<Systems>();
            using (ListProvider provider = new ListProvider())
            {
                system = provider.GetSystemsListCh(GlobalVars.BadBeeFilter);
                system.AddRange(provider.GetSystemsList(GlobalVars.BadBeeFilter));
            }
            return system.Where(q => q.SystemId != 0).Count() > 0;
        }

        public bool HasWvaFilterAnyElements()
        {
            List<Wva> wva = new List<Wva>();
            using (ListProvider provider = new ListProvider())
            {
                wva = provider.GetWvaChList(GlobalVars.BadBeeFilter);
                wva.AddRange(provider.GetWvaList(GlobalVars.BadBeeFilter));
            }
            return wva.Where(q => q.WvaId != 0).Count() > 0;
        }
        
        public bool HasBadBeesFilterAnyElements()
        {
            List<DAL.BadBee> BadBees = new List<DAL.BadBee>();
            using (ListProvider provider = new ListProvider())
            {
                BadBees = provider.GetBadBeeChList(GlobalVars.BadBeeFilter);
                BadBees.AddRange(provider.GetBadBeeList(GlobalVars.BadBeeFilter));
            }
            return BadBees.Where(q => q.BadBeeId != 0).Count() > 0;
        }

        public bool HasBrandsFilterAnyElements()
        {
            List<Brand> brands = new List<Brand>();
            using (ListProvider provider = new ListProvider())
            {
                brands = provider.GetBrandsChList(GlobalVars.BadBeeFilter);
                brands.AddRange(provider.GetBrandsList(GlobalVars.BadBeeFilter));
            }
            return brands.Where(q => string.IsNullOrEmpty(q.Name) == false).Count() > 0;
        }

        public bool HasSeriesFilterAnyElements()
        {
            List<Serie> series = new List<Serie>();
            using (ListProvider provider = new ListProvider())
            {
                series = provider.GetSeriesChList(GlobalVars.BadBeeFilter);
                series.AddRange(provider.GetSeriesList(GlobalVars.BadBeeFilter));
            }
            return series.Where(q => string.IsNullOrEmpty(q.Name) == false).Count() > 0;
        }

        public bool HasModelsFilterAnyElements()
        {
            List<Model> models = new List<Model>();
            using (ListProvider provider = new ListProvider())
            {
                models = provider.GetModelsChList(GlobalVars.BadBeeFilter);
                models.AddRange(provider.GetModelsList(GlobalVars.BadBeeFilter));
            }
            return models.Where(q => string.IsNullOrEmpty(q.Name) == false).Count() > 0;
        }
    }
}