# Overview

Xamflix is a try to clone Netflix functionality using Xamarin.Forms that was initially developed as a demo project for .NET Conf Armenia 2021. The purpose of the demo was the following

- Prove on practice the benefits of using Xamarin.Form to build cross platform mobile applications
- Write as few code as possible yet achieve something functional and beautiful
- Demonstrate the UI design power that even Xamarin.Forms posses, to prove that it’s underestimated many times
- Demonstrate some good practices of Xamarin architecture in general

## The idea

Idea is very simple - try to mimic Netflix as much as possible, using as few platform specific code as possible. One of the main requirements is implementing offline-first application, so that when there's no internet connection, app will be usable. To achieve that required, MongoDB together with Realm Cloud Sync and Realm .NET SDK was used.

## How to run the sample

1. Deploy MongoDB Realm cluster following the getting started guide from [here](https://www.mongodb.com/realm)
2. [Enable](https://docs.mongodb.com/realm/sync/get-started#std-label-enable-sync) Realm Cloud Sync
3. Enable API Key authentication and generate a new API Key.
4. Replace `Realm:ApiKey` section within the [appsettings.json](./src/Backend/Xamflix.MediaProcessor/appsettings.json) and also [Forms appsettings.json](./src/App/Xamflix.App.Forms/appsettings.json)
    - You can also use user secrets to replace the keys as well.
5. Deploy Azure Media Services account following [quick start guide](https://docs.microsoft.com/en-us/azure/media-services/latest/how-to-set-azure-subscription?tabs=portal)
   - For me, the best approach was creating Azure Media Service account in Azure portal, then following [this guide](https://docs.microsoft.com/en-us/azure/media-services/latest/access-api-howto?tabs=cli) to create a service principal. Sorry, ARM templates and proper INFRA deployment will come later
6. After the service principle JSON is generated, replace the `MediaService` section within the [appsettings.json](./src/Backend/Xamflix.MediaProcessor/appsettings.json) with it.
7. Download the sample Movie database from [here](https://1drv.ms/u/s!AtQTsqbP2B9Rk9xQUqW2lDTnDng-3g?e=rjeAFW) and place it somewhere near the cloned project
8. Copy the root path of the Movies folder and place it inside `Import:ImportDir` configuration section within [appsettings.json](./src/Backend/Xamflix.MediaProcessor/appsettings.json)
9. Run the sample and waaaaaait. It will take about an hour or two for the videos to be processed and encoded into streams.
10. YOU ARE READY TO RUN THE APP!! Yeaaah.
11. Select the target project and run as any Xamarin project you would.