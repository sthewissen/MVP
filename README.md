# MVP

There has always been the need for Microsoft MVPs to manage their community activities in an easier way. The online portal works fine, but having a similar thing in your pocket to create activities on the fly is what we really need. Which is what this project is for!

The idea here is that with a great community full of Xamarin / Microsoft MVPs it shouldn't be too hard to come up with something fancy, right? Since it would have to happen in a somewhat streamlined fashion though it would be nice to focus the work in a single GitHub repo, which is what you see here.

How do I contribute?
--
If you want to pitch in, take a look at the issues currently filed, which should contain some of the basic functionalities the app should be able to perform. Does one of them appeal to you? Go ahead and pick it up, let me know and submit a PR when you're done!

MVP API
--
There is a current MVP API made available at: https://mvpapi.portal.azure-api.net/.

It exposes methods for the following:

- `GET` GetContributionAreas
- `GET` GetContributionById
- `GET` GetContributions
- `GET` GetContributionTypes
- `POST` PostContribution
- `PUT` PutContribution
- `DELETE` DeleteContribution
- `GET` GetOnlineIdentities
- `GET` GetOnlineIdentitiesByNominationsId
- `POST` PostOnlineIdentity
- `PUT` PutOnlineIdentity
- `DELETE` DeleteOnlineIdentity
- `GET` GetMVPProfile
- `GET` GetMVPProfileById
- `GET` GetMVPProfileImage
- `GET` GetSharingPreferences
- `GET` GetCurrentQuestions
- `GET` GetAnswers
- `POST` SaveAnswers
- `POST` SubmitAnswers

From this the following basic functionalities can be distilled:

- Manage contributions (delete, update, add)
- Manage your online identities
- Search for other MVPs
- View your own profile (not edit unfortunately)
- Save or submit answers to MVP renewal questions

Some other nice to haves we could come up with:
- Local reminder notifications to fill in your contributions
- ...

So, who's with me?
--

As far as scopes this seems fairly simple, but since this is what we have to work with I think it could also be a really nice little app to build as a community! And maybe even more importantly, help MVPs across to globe!
