[![NuGet package](https://img.shields.io/nuget/v/TruePeople.SharePreview.v7)](https://www.nuget.org/packages/TruePeople.SharePreview.v7)

# TruePeople.SharePreview.v7
Share preview URLs with non-Umbraco users! This project has been backported by Rhythm to Umbraco 7 from the original [TruePeople.SharePreview](https://github.com/TruePeople/TruePeople.SharePreview) plugin for Umbraco 8.

## Installation
Install the package via [NuGet](https://www.nuget.org/packages/TruePeople.SharePreview.v7) or clone this repository and build a local NuGet package yourself.


## Settings
In the settings section you will see a new tree node 'Shareable Preview settings', here you can manage the encryption key that will be used to generate the shareable preview URL.
You can also manage the URL a user will get redirected to when the link isn't valid anymore.

## How to share a preview?
After installation you will see a new button next to the 'Preview' button on your content nodes.
**When the newest version is published, this button will be disabled.**
Clicking on this button will directly copy the link to your clipboard.

## How long will the link be valid for?
The link generated will remain valid until the version that the link was generated on has been published.
When someone tries to access the link then, they will be redirected to the URL you configured in the settings.

## How does it work?
The generated link consists of the node ID and a version ID of the preview version.
We render the correct content by using a custom route and having our own UmbracoVirtualNodeRouteHandler implemented.
The handler takes care of checking if the link is still valid and for setting the preview content to the current request.

## How to run this on your local machine?
Clone this repository to your machine.
Run the build.ps1 script that is located in the package folder.
Specify what version you would like to give the build, and if you want to package it up for Umbraco or NuGet.
This will generate the packages inside the same folder.


# Changelog

## v1.0.2

- Fixed null exception bug when checking if a shareable link exists for content without a template

## v1.0.1

- Fixed bug where switching tabs in the backoffice would cause the share button to be hidden
- Changed package build files to copy release DLLs instead of debug ones

## v1.0.0

- Initial package release for Umbraco v7
	
---
