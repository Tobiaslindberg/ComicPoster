using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ComicPoster.Common;
using ComicPoster.Slack;

namespace ComicPoster
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var providerTypes = GetProviders();

            var slackClient = new SlackClient();

            var tableService = GetTableService();

            foreach (var providerType in providerTypes)
            {
                Console.Out.WriteLine($"Running provider: {providerType.Name}");
                var provider = (IComicProvider) Activator.CreateInstance(providerType);

                var providerEntity = tableService.GetProviderEntity(providerType.Name);

                var comic = provider.DownloadComic(providerEntity?.LastId ?? string.Empty);

                if (comic == null)
                {
                    Console.Out.WriteLine($"Found no new comic from provider {providerType.Name}");

                    continue;
                }

                Console.Out.WriteLine($"Found a new comic from provider {providerType.Name}");
                var message = CreateMessage(comic);
                var messageStatus = slackClient.Post(message);

                if (messageStatus)
                {
                    Console.Out.WriteLine("Successfully posted comic to slack");
                    tableService.UpdateProviderEntity(providerEntity, providerType.Name, comic.Id);
                    Console.Out.WriteLine("Successfully updated last id in Azure Table");
                }
            }
        }

        private static ITableService GetTableService()
        {
            ITableService tableService;
            if (SettingsHelper.UseTableService)
            {
                tableService = new CloudTableService();
            }
            else
            {
                tableService = new NullTableService();
            }
            return tableService;
        }

        private static IEnumerable<Type> GetProviders()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(path))
            {
                Console.Error.WriteLine("Couldn't get current executing path");

                throw new DirectoryNotFoundException("Could not find the path of current application");
            }

            var allAssemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFile).ToList();

            return allAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof (IComicProvider).IsAssignableFrom(p) && !p.IsInterface)
                .ToList();
        }

        private static Message CreateMessage(Comic comic)
        {
            var message = new Message
            {
                channel = SettingsHelper.Channel,
                Username = "Comic poster",
                Icon_emoji = ":newspaper:",
                Mrkdwn = true,
                Attachments = new List<Attachment>()
            };

            foreach (var comicImage in comic.ComicImages)
            {
                var attachment = new Attachment
                {
                    image_url = comicImage.ImageUrl.ToString().Replace(" ", "%20"),
                    text = $"{comicImage.AltText}",
                    author_link = comic.ComicUrl.ToString(),
                    author_name = $"{comic.Name}",
                    title = comic.Title,
                    title_link = comic.PermaLink.ToString()
                };

                message.Attachments.Add(attachment);
            }
            return message;
        }

        private static IEnumerable<Type> GetProvidersFromPluginFolder()
        {
            var currentDirectory = Thread.GetDomain().BaseDirectory;
            var pluginsPath = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\Plugins"));
            var pluginFiles = new DirectoryInfo(pluginsPath).GetFiles();

            var assemblies = pluginFiles
                .Select(fileInfo => Assembly.LoadFrom(fileInfo.FullName))
                .ToList();

            var providerTypes = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof (IComicProvider).IsAssignableFrom(p) && !p.IsInterface);

            return providerTypes;
        }
    }
}