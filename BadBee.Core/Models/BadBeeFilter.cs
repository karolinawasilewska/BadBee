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
                    + ";" + this.Widths + ";" + this.Heights + ";" + this.Thicknesses
                    + ";" + this.Systems + ";" + this.PhraseFilter;

                return Base64Encode(key);
            }
        }

        private string ReadList(List<int> list)
        {
            if (list == null)
                list = new List<int>();

            return string.Join("|", list.Distinct().ToList());
        }

        private List<int> FillList(List<int> list, string value)
        {
            if (list == null)
                list = new List<int>();
            if (value != null)
            {
                string[] tab = value.Split('|');
                foreach (var item in tab)
                {
                    int iitem = int.Parse(item);
                    if (list.Count(q => q == iitem) == 0)
                    {
                        list.Add(iitem);
                    }
                }
            }

            return list.Distinct().ToList();
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
        public List<int> BrandsList { get;  set; }
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
        public List<int> SeriesList { get;  set; }

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
        public List<int> ModelsList { get;  set; }
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
        public List<int> DateYearsList { get; set; }

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
        public List<int> BadBeeNumbersList { get;  set; }

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
        public List<int> WvasList { get; set; }
        
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
        public List<int> WidthsList { get; set; }

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
        public List<int> HeightsList { get; set; }

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
        public List<int> ThicknessesList { get; set; }

     

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
        public List<int> SystemsList { get; set; }

        public string PhraseFilter { get; set; }

       

        public bool BrandsSelected { get { return IsFilterSelected(this.Brands); } }
        public bool SeriesSelected { get { return IsFilterSelected(this.Series); } }
        public bool ModelsSelected { get { return IsFilterSelected(this.Models); } }
        public bool DateYearsSelected { get { return IsFilterSelected(this.DateYears); } }
        public bool BadBeeNumbersSelected { get { return IsFilterSelected(this.BadBeeNumbers); } }
        public bool WvasSelected { get { return IsFilterSelected(this.Wvas); } }
        public bool WidthsSelected { get { return IsFilterSelected(this.Widths); } }
        public bool HeightsSelected { get { return IsFilterSelected(this.Heights); } }
        public bool ThicknessesSelected { get { return IsFilterSelected(this.Thicknesses); } }
        public bool SystemsSelected { get { return IsFilterSelected(this.Systems); } }
        public bool PhraseFilterSelected { get { return IsFilterSelected(this.PhraseFilter); } }
        public bool StatusSelected { get { return IsFilterSelected(this.Status); } }
        public bool JobSelected { get { return IsFilterSelected(this.Job); } }
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