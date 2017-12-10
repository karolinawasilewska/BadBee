using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadBee.Core.Models
{
    public class BadBeeFilter
    {

        public bool IsFilterEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.Brands) && string.IsNullOrEmpty(this.Series) && string.IsNullOrEmpty(this.Models)
                    && string.IsNullOrEmpty(this.DateYears) && string.IsNullOrEmpty(this.BadBeeNumbers) && string.IsNullOrEmpty(this.Wvas)
                    && string.IsNullOrEmpty(this.WvaDetails2) && string.IsNullOrEmpty(this.DrumDiameters) && string.IsNullOrEmpty(this.Rivets)
                    && string.IsNullOrEmpty(this.Widths) && string.IsNullOrEmpty(this.Heights) && string.IsNullOrEmpty(this.Thicknesses)
                    && string.IsNullOrEmpty(this.Systems) && string.IsNullOrEmpty(this.PhraseFilter);
            }
        }

        public string SearchKey
        {
            get
            {
                string key = this.Brands + ";" + this.Series + ";" + this.Models
                    + ";" + this.DateYears + ";" + this.BadBeeNumbers + ";" + this.Wvas
                    + ";" + this.WvaDetails2 + ";" + this.DrumDiameters + ";" + this.Rivets
                    + ";" + this.Widths + ";" + this.Heights + ";" + this.Thicknesses
                    + ";" + this.Systems + ";" + this.PhraseFilter;

                return Base64Encode(key);
            }
        }

        private string ReadList(List<string> list)
        {
            if (list == null)
                list = new List<string>();

            return string.Join("|", list.Distinct().ToList());
        }

        private List<string> FillList(List<string> list, string value)
        {
            if (list == null)
                list = new List<string>();
            if (value != null)
            {
                string[] tab = value.Split('|');
                foreach (var item in tab)
                {
                    if (list.Count(q => q == item) == 0)
                    {
                        list.Add(item);
                    }
                }
            }

            return list.Where(q => !string.IsNullOrEmpty(q)).Distinct().ToList();
        }

        public string Brands
        {
            get
            {
                return ReadList(BrandsList);
            }
            set
            {
                BrandsList = FillList(BrandsList, value);
            }
        }
        public List<string> BrandsList { get;  set; }
        public string Status { get; set; }
        public string Job { get; set; }
        public int? Page { get; set; }
        public int? TotalCount { get; set; }

        public string Series
        {
            get
            {
                return ReadList(SeriesList);
            }
            set
            {
                SeriesList = FillList(SeriesList, value);
            }
        }
        public List<string> SeriesList { get;  set; }

        public string Models
        {
            get
            {
                return ReadList(ModelsList);
            }
            set
            {
                ModelsList = FillList(ModelsList, value);
            }
        }
        public List<string> ModelsList { get;  set; }
        public string DateYears
        {
            get
            {
                return ReadList(DateYearsList);
            }
            set
            {
                DateYearsList = FillList(DateYearsList, value);
            }
        }
        public List<string> DateYearsList { get; private set; }

        public string BadBeeNumbers
        {
            get
            {
                return ReadList(BadBeeNumbersList);
            }
            set
            {
                BadBeeNumbersList = FillList(BadBeeNumbersList, value);
            }
        }
        public List<string> BadBeeNumbersList { get;  set; }

        public string Wvas
        {
            get
            {
                return ReadList(WvasList);
            }
            set
            {
                WvasList = FillList(WvasList, value);
            }
        }
        public List<string> WvasList { get; set; }

        public string WvaDetails2
        {
            get
            {
                return ReadList(WvasDetailsList);
            }
            set
            {
                WvasDetailsList = FillList(WvasDetailsList, value);
            }
        }
        public List<string> WvasDetailsList { get;  set; }

        public string Widths
        {
            get
            {
                return ReadList(WidthsList);
            }
            set
            {
                WidthsList = FillList(WidthsList, value);
            }
        }
        public List<string> WidthsList { get; private set; }

        public string Heights
        {
            get
            {
                return ReadList(HeightsList);
            }
            set
            {
                HeightsList = FillList(HeightsList, value);
            }
        }
        public List<string> HeightsList { get; private set; }

        public string Thicknesses
        {
            get
            {
                return ReadList(ThicknessesList);
            }
            set
            {
                ThicknessesList = FillList(ThicknessesList, value);
            }
        }
        public List<string> ThicknessesList { get; private set; }

        public string Rivets
        {
            get
            {
                return ReadList(RivetsList);
            }
            set
            {
                RivetsList = FillList(RivetsList, value);
            }
        }
        public List<string> RivetsList { get; private set; }

        public string DrumDiameters
        {
            get
            {
                return ReadList(DrumDiametersList);
            }
            set
            {
                DrumDiametersList = FillList(DrumDiametersList, value);
            }
        }
        public List<string> DrumDiametersList { get; private set; }

        public string Systems
        {
            get
            {
                return ReadList(SystemsList);
            }
            set
            {
                SystemsList = FillList(SystemsList, value);
            }
        }
        public List<string> SystemsList { get; private set; }

        public string PhraseFilter { get; set; }

        public string CrossName { get; set; }

        public string CrossNumbers {
            get
            {
                return ReadList(CrossList);
            }
            set
            {
                CrossList = FillList(CrossList, value);
            }
        }
        public List<string> CrossList { get; set; }

        public bool BrandsSelected { get { return IsFilterSelected(this.Brands); } }
        public bool SeriesSelected { get { return IsFilterSelected(this.Series); } }
        public bool ModelsSelected { get { return IsFilterSelected(this.Models); } }
        public bool DateYearsSelected { get { return IsFilterSelected(this.DateYears); } }
        public bool BadBeeNumbersSelected { get { return IsFilterSelected(this.BadBeeNumbers); } }
        public bool WvasSelected { get { return IsFilterSelected(this.Wvas); } }
        public bool WvaDetails2Selected { get { return IsFilterSelected(this.WvaDetails2); } }
        public bool RivetsSelected { get { return IsFilterSelected(this.Rivets); } }
        public bool WidthsSelected { get { return IsFilterSelected(this.Widths); } }
        public bool HeightsSelected { get { return IsFilterSelected(this.Heights); } }
        public bool ThicknessesSelected { get { return IsFilterSelected(this.Thicknesses); } }
        public bool DrumDiametersSelected { get { return IsFilterSelected(this.DrumDiameters); } }
        public bool SystemsSelected { get { return IsFilterSelected(this.Systems); } }
        public bool PhraseFilterSelected { get { return IsFilterSelected(this.PhraseFilter); } }
        public bool StatusSelected { get { return IsFilterSelected(this.Status); } }
        public bool JobSelected { get { return IsFilterSelected(this.Job); } }
        public bool CrossSelected { get { return IsFilterSelected(this.CrossName); } }
        private bool IsFilterSelected(string filter)
        {
            return string.IsNullOrEmpty(filter) == false;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}