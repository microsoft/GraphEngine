---
id: vs-extension
title: Visual Studio Extension
permalink: /docs/manual/vs-extension.html
---

We briefly introduce the usage of {{site.fullname}} [Visual Studio Extension](https://visualstudiogallery.msdn.microsoft.com/12835dd2-2d0e-4b8e-9e7e-9f505bb909b8) in this chapter.

We usually start with creating a _{{site.fullname}} Data Modeling Project_. In the
Visual Studio _New Project_ dialog, select _{{site.fullname}} Data Modeling Project_
project template, make sure _.NET Framework 4.5_ is chosen, and
provide a project name.

<img src="/img/vs/tsl-newproject.png" style="padding-top: 20px; margin-left:10em;width:55em;"></img>

After clicking the _OK_ button, we have a _Data Modeling Project_
in the _Solution Explorer_:

<img src="/img/vs/solution-1.png" style="padding-top: 20px; margin-left:10em;width:25em;"></img>

As shown below, now we can add a _{{site.fullname}} Application
Project_ using the visual studio _Add New Project_ dialog.

<img src="/img/vs/app-newproject.png" style="padding-top: 20px; margin-left:10em;width:55em;"></img>

After clicking the _OK_ button, a project wizard shows up to help us
create a customized application project.

<img src="/img/vs/wizard-1.png" style="padding-top: 20px; margin-left:10em;width:55em;"></img>

It will first let us choose which data modeling (TSL) project we want
to reference.

<img src="/img/vs/wizard-2.png" style="padding-top: 20px; margin-left:10em;width:55em;"></img>

Then, the wizard will ask us whether we want the wizard to help
generate the message passing stub code.

<img src="/img/vs/wizard-3.png" style="padding-top: 20px; margin-left:10em;width:55em;"></img>

After clicking the _Finish_ button, a customized _{{site.fullname}}
application_ project is added. And we are ready to do the development
now.

<img src="/img/vs/solution-2.png" style="padding-top: 20px; margin-left:10em;width:25em;"></img>
