using Lumag.Core.DAL;
using Lumag.Core.Models;
using Lumag.Core.MyResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lumag.Core.Providers
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
                DataTarget = "#lumagFilterModal",
                Description = @Resources.lumag_no,
                ID = "lumagnumber",
                OnClickMethod = "showLumag()",                
                PictureSrc = "../../Images/lumag_head_tran.png",
                IsFilterSelected = GlobalVars.LumagFilter.LumagNumbersSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasLumagsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#brandFilterModal",
                Description = @Resources.brand,
                ID = "brand",
                OnClickMethod = "showBrands()",
               // PictureSrc = null,
                PictureSrc = "../../Images/nothing_head.png",
                IsFilterSelected = GlobalVars.LumagFilter.BrandsSelected,
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
                //PictureSrc = null,
                PictureSrc = "../../Images/nothing_head.png",
                IsFilterSelected = GlobalVars.LumagFilter.SeriesSelected,
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
                //PictureSrc = null,
                PictureSrc = "../../Images/nothing_head.png",
                IsFilterSelected = GlobalVars.LumagFilter.ModelsSelected,
                CssStyle = "border-right:0 !important;",
                HasAnyElements = HasModelsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                ID = "kwkmmod",
                CssClass = "kw_km",
                Description = @Resources.power,
                //PictureSrc = null,
                PictureSrc = "../../Images/nothing_head.png",
                Colspan = "2"
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#yearsFilterModal",
                Description = @Resources.year,
                ID = "years",
                OnClickMethod = "showYears()",
               // PictureSrc = null,
                PictureSrc = "../../Images/year_head_tran.png",
                IsFilterSelected = GlobalVars.LumagFilter.DateYearsSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasYearsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "fr",
                Description = @Resources.front_rear,
                //PictureSrc = null,
                PictureSrc = "../../Images/fr_head_tran.png"
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "wva",
                Description = @Resources.wva,
                PictureSrc = "../../Images/wva_head_tran.png",
                Colspan = "4"
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "size",
                Description = @Resources.size,
                PictureSrc = "../../Images/size_head_tran.png",
                Colspan = "3"
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "wedge",
                Description = @Resources.wedge,
                PictureSrc = "../../Images/wedge_head_tran.png"
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#diameterFilterModal",
                Description = @Resources.diameter,
                ID = "drum",
                OnClickMethod = "showDrumDiameters()",
                PictureSrc = "../../Images/drum_head_tran.png",
                IsFilterSelected = GlobalVars.LumagFilter.DrumDiametersSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasDrumsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.TopFilter,
                DataTarget = "#systemFilterModal",
                Description = @Resources.system,
                ID = "sys",
                OnClickMethod = "showSystems()",
                PictureSrc = "../../Images/brake_head_tran.png",
                IsFilterSelected = GlobalVars.LumagFilter.SystemsSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasSystemsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.TopEmpty,
                CssClass = "rivets",
                CssStyle = "border-right:0 !important;",
                Description = @Resources.rivets,
                PictureSrc = "../../Images/rivets_head_tran.png",
                Colspan = "2"
            });

            return items;
        }
        public List<CatalogContentTableHeader> CreateDetailsHeaderItems()
        {
            List<CatalogContentTableHeader> items = new List<CatalogContentTableHeader>();

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Colspan = "4"
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Description = Resources.kw
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Description = Resources.km
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Colspan = "2"
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
                IsFilterSelected = GlobalVars.LumagFilter.WvasSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasWvaFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Description = Resources.qtt
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#wvaDetFilterModal",
                Description = @Resources.details,
                ID = "wvadetails",
                OnClickMethod = "showWvaDet()",
                IsFilterSelected = GlobalVars.LumagFilter.WvaDetails2Selected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasWvaDetFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#heightFilterModal",
                Description = @Resources.h,
                ID = "height",
                OnClickMethod = "showHeights()",
                IsFilterSelected = GlobalVars.LumagFilter.HeightsSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasHeightsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#widthFilterModal",
                Description = @Resources.w,
                ID = "width",
                OnClickMethod = "showWidths()",
                IsFilterSelected = GlobalVars.LumagFilter.WidthsSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasWidthsFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#thicknessFilterModal",
                Description = @Resources.t,
                ID = "thickness",
                OnClickMethod = "showThicknesses()",
                IsFilterSelected = GlobalVars.LumagFilter.ThicknessesSelected,
                CssStyle = "cursor: pointer;",
                HasAnyElements = HasThicknessesFilterAnyElements()
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Colspan = "3"
            });

            items.Add(new CatalogContentTableHeader()
            {
                Type = CatalogContentTableHeaderType.DetailsEmpty,
                Description = Resources.qtt
            });

            items.Add(new CatalogContentTableHeader()
            {
                DataToggle = "modal",
                Type = CatalogContentTableHeaderType.DetailsFilter,
                DataTarget = "#rivetFilterModal",
                Description = @Resources.type,
                ID = "riv",
                OnClickMethod = "showRivetTypes()",
                IsFilterSelected = GlobalVars.LumagFilter.RivetsSelected,
                CssStyle = "cursor: pointer;border-right:0 !important;",
                HasAnyElements = HasRivetsFilterAnyElements()
            });

            return items;
        }

        public bool HasYearsFilterAnyElements()
        {            
            List<Years> year = new List<Years>();
            using (ListProvider provider = new ListProvider())
            {
                year = provider.GetYearsListCh(GlobalVars.LumagFilter);
                year.AddRange(provider.GetYearsList(GlobalVars.LumagFilter));
            }
            return year.Where(q => q.Id != 0).Count() > 0;
        }
        
        public bool HasHeightsFilterAnyElements()
        {            
            List<Heights> hei = new List<Heights>();
            using (ListProvider provider = new ListProvider())
            {
                hei = provider.GetHeightsListCh(GlobalVars.LumagFilter);
                hei.AddRange(provider.GetHeightsList(GlobalVars.LumagFilter));
            }
            return hei.Where(q => q.Id != 0).Count() > 0;
        }
       
        public bool HasWidthsFilterAnyElements()
        {            
            List<Widths> wid = new List<Widths>();
            using (ListProvider provider = new ListProvider())
            {
                wid = provider.GetWidthsListCh(GlobalVars.LumagFilter);
                wid.AddRange(provider.GetWidthsList(GlobalVars.LumagFilter));
            }
            return wid.Where(q => q.Id != 0).Count() > 0;
        }
        
        public bool HasThicknessesFilterAnyElements()
        {            
            List<Thicknesses> thick = new List<Thicknesses>();
            using (ListProvider provider = new ListProvider())
            {
                thick = provider.GetThicknessesListCh(GlobalVars.LumagFilter);
                thick.AddRange(provider.GetThicknessesList(GlobalVars.LumagFilter));
            }
            return thick.Where(q => q.Id != 0).Count() > 0;
        }
        
        public bool HasDrumsFilterAnyElements()
        {            
            List<DrumDiameters> drums = new List<DrumDiameters>();
            using (ListProvider provider = new ListProvider())
            {
                drums = provider.GetDrumDiametersListCh(GlobalVars.LumagFilter);
                drums.AddRange(provider.GetDrumDiametersList(GlobalVars.LumagFilter));
            }
            return drums.Where(q => q.Id != 0).Count() > 0;
        }
        
        public bool HasSystemsFilterAnyElements()
        {            
            List<Systems> system = new List<Systems>();
            using (ListProvider provider = new ListProvider())
            {
                system = provider.GetSystemsListCh(GlobalVars.LumagFilter);
                system.AddRange(provider.GetSystemsList(GlobalVars.LumagFilter));
            }
            return system.Where(q => q.Id != 0).Count() > 0;
        }
        
        public bool HasRivetsFilterAnyElements()
        {            
            List<RivetTypes> rivets = new List<RivetTypes>();
            using (ListProvider provider = new ListProvider())
            {
                rivets = provider.GetRivetListCh(GlobalVars.LumagFilter);
                rivets.AddRange(provider.GetRivetTypesList(GlobalVars.LumagFilter));
            }
            return rivets.Where(q=>q.Id != 0).Count() > 0;
        }
        
        public bool HasWvaFilterAnyElements()
        {            
            List<Wva> wva = new List<Wva>();
            using (ListProvider provider = new ListProvider())
            {
                wva = provider.GetWvaChList(GlobalVars.LumagFilter);
                wva.AddRange(provider.GetWvaList(GlobalVars.LumagFilter));
            }
            return wva.Where(q => q.Id != 0).Count() > 0;
        }
        
        public bool HasWvaDetFilterAnyElements()
        {            
            List<WvaDetails> wvadet = new List<WvaDetails>();
            using (ListProvider provider = new ListProvider())
            {
                wvadet = provider.GetWvaDetailsChList(GlobalVars.LumagFilter);
                wvadet.AddRange(provider.GetWvaDetList(GlobalVars.LumagFilter));
            }
            return wvadet.Where(q => q.Id != 0).Count() > 0;
        }
        
        public bool HasLumagsFilterAnyElements()
        {
            List<LumagNumbers> lumags = new List<LumagNumbers>();
            using (ListProvider provider = new ListProvider())
            {
                lumags = provider.GetLumagChList(GlobalVars.LumagFilter);
                lumags.AddRange(provider.GetLumagList(GlobalVars.LumagFilter));
            }
            return lumags.Where(q=>q.LumagNumberId != 0).Count() > 0;
        }
       
        public bool HasBrandsFilterAnyElements()
        {            
            List<Brands> brands = new List<Brands>();
            using (ListProvider provider = new ListProvider())
            {
                brands = provider.GetBrandsChList(GlobalVars.LumagFilter);
                brands.AddRange(provider.GetBrandsList(GlobalVars.LumagFilter));
            }
            return brands.Where(q => string.IsNullOrEmpty(q.Name) == false).Count() > 0;
        }
       
        public bool HasSeriesFilterAnyElements()
        {            
            List<Series> series = new List<Series>();
            using (ListProvider provider = new ListProvider())
            {
                series = provider.GetSeriesChList(GlobalVars.LumagFilter);
                series.AddRange(provider.GetSeriesList(GlobalVars.LumagFilter));
            }
            return series.Where(q => string.IsNullOrEmpty(q.Name) == false).Count() > 0;
        }
        
        public bool HasModelsFilterAnyElements()
        {            
            List<DAL.Models> models = new List<DAL.Models>();
            using (ListProvider provider = new ListProvider())
            {
                models = provider.GetModelsChList(GlobalVars.LumagFilter);
                models.AddRange(provider.GetModelsList(GlobalVars.LumagFilter));
            }
            return models.Where(q => string.IsNullOrEmpty(q.Name) == false).Count() > 0;
        }
    }
}