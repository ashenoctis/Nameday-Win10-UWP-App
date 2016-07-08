using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundServices
{
    public sealed class MyBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var settings = new Settings();

            if (settings.NotificationsEnabled)
                await SendNotificationAsync();

            if (settings.UpdatingLiveTileEnabled)
                await UpdateTilesAsync();

            _deferral.Complete();
        }

        private async Task SendNotificationAsync()
        {
            var todaysNames = await NamedayRepository.GetTodaysNamesAsStringAsync();
            if (todaysNames == null)
                return;

            ToastNotifier notifier = ToastNotificationManager.CreateToastNotifier();
            XmlDocument content = ToastNotificationManager.GetTemplateContent(
                    ToastTemplateType.ToastText02);

            var texts = content.GetElementsByTagName("text");

            texts[0].InnerText = todaysNames.Contains(",") ?
                "Today's namedays are" : "Today's nameday is";

            texts[1].InnerText = todaysNames;

            notifier.Show(new ToastNotification(content));
        }

        private async static Task UpdateTilesAsync()
        {
            var todaysNames = await NamedayRepository.GetTodaysNamesAsStringAsync();
            if (todaysNames == null)
                return;

            var template =
@"<tile>
  <visual version=""4"">
    <binding template=""TileMedium"">
	  <text hint-wrap=""true"">{0}</text>
	</binding>
    <binding template=""TileWide"">
	  <text hint-wrap=""true"">{0}</text>
	</binding>
  </visual>
</tile>";

            var content = string.Format(template, todaysNames);
            var doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(content);

            TileUpdateManager.CreateTileUpdaterForApplication().
                Update(new TileNotification(doc));

        }

        public static async void Register()
        {
            var isRegistered = BackgroundTaskRegistration.AllTasks.Values.Any(
                t => t.Name == nameof(MyBackgroundTask));

            if (isRegistered)
                return;

            if (await BackgroundExecutionManager.RequestAccessAsync()
                == BackgroundAccessStatus.Denied)
                return;


            var builder = new BackgroundTaskBuilder
            {
                Name = nameof(MyBackgroundTask),
                TaskEntryPoint = $"{nameof(BackgroundServices)}.{nameof(MyBackgroundTask)}"
            };

            builder.SetTrigger(new TimeTrigger(120, false));

            builder.Register();
        }
    }
}
