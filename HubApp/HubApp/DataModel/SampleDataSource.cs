using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace HubApp.Data
{
    // Sample Data Item Class
    public class SampleDataItem
    {
        public SampleDataItem(String title, String subtitle, String content)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Content = content;
        }

        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Content { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    // Sample Data Group Class
    public class SampleDataGroup
    {
        public SampleDataGroup(String title, String subtitle)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.Title.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.Title.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                SampleDataGroup group = new SampleDataGroup(groupObject["Title"].GetString(),
                                                            groupObject["Subtitle"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new SampleDataItem(itemObject["Title"].GetString(),
                                                       itemObject["Subtitle"].GetString(),
                                                       itemObject["Content"].GetString()));
                }
                this.Groups.Add(group);
            }
        }
    }
}