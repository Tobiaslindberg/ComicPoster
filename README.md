# Comic Poster for [Slack](https://slack.com/)
Comic Poster is an c# application that fetches comics from different providers and posts them to your preferred channel on Slack.
Comic Poster is built to be run in [Azure](https://azure.microsoft.com/en-us/) as an [Azure WebJob](https://azure.microsoft.com/en-us/services/app-service/) with [Azure Table Storage](https://azure.microsoft.com/en-us/services/storage/tables/) for storage and a [Azure Scheduler](https://azure.microsoft.com/en-us/services/scheduler/) for triggering the WebJob.

Currently there are support for these comics:
* [Dilbert](http://dilbert.com/) ([ComicPoster.Dilbert](ComicPoster.Dilbert))
* [Saturday Morning Breakfast Cereal](http://www.smbc-comics.com/) ([ComicPoster.Smbc](ComicPoster.Smbc))
* [XKCD](https://xkcd.com/) ([ComicPoster.Xkcd](ComicPoster.Xkcd))