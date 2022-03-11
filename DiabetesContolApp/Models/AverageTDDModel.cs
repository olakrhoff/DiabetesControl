using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace DiabetesContolApp.Models
{
    [Table("AverageTDD")]
    public class AverageTDDModel : IModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int AverageTDDID { get; set; }
        [NotNull]
        public long DateTimeLong { get; set; } //When the change happened
        [NotNull]
        public float AverageTDDValue { get; set; }

        public AverageTDDModel()
        {
        }

        public AverageTDDModel(float averageTDDValue)
        {
            DateTimeValue = DateTime.Now;
            AverageTDDValue = averageTDDValue;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Ignore]
        public DateTime DateTimeValue
        {
            get
            {
                return DateTime.FromBinary(this.DateTimeLong);
            }

            set
            {
                if (value.ToBinary() != this.DateTimeLong)
                {
                    this.DateTimeLong = value.ToBinary();
                    OnPropertyChanged();
                }
            }
        }

        public string ToStringCSV()
        {
            return AverageTDDID + ", " +
                DateTimeValue.ToString("yyyy/MM/dd HH:mm") + ", " +
                AverageTDDValue + "\n";
        }
    }
}
