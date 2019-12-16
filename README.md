# Internet-Inc

## Testing Instructions

Thank you for helping me test my game for my thesis! I've tried
to include some instructions / guidance below, but feel free to
reach out if anything is unclear. Please submit all feedback via
my Google Form listed below. Submitting issues involving known
bugs is welcome and encouraged, as it helps remind me of these
issues.

Google Form: https://forms.gle/aqkHKJVdyVtxvsWW8

## Getting Started

If you haven't already downloaded the game, it is currently
available at https://people.cs.ksu.edu/~rclafferty/Games/Thesis
for Windows 64 bit and Mac OSX. If you need to request any
other versions or are having issues playing the game, please
send that in writing and I will fix that ASAP.

Once the game is downloaded, there should be no installation
and no special administrative permissions required.

## Background

The game Internet, Inc. is built around the idea of Domain Name
System (DNS) Lookup, a protocol in the internet that aids in
translating URLs to physical web servers. I'll give an example
using the URL www.google.com .

If you type in www.google.com into your web browser, it goes
through the DNS Lookup process in order to find the correct
server for that URL. The process looks something like this:

Your computer starts by asking a root server (main server) where
www.google.com is located. It responds with "ask the guy that
keeps track of all *.com URLs".

Your computer then asks the *.com server where www.google.com is
located. It responds with "ask the guy that keeps track of all
*.google.com URLs".

Your computer then asks the *.google.com server where
www.google.com is located. It responds with the address of the
correct web server. Only after this process is complete does
your computer request the www.google.com homepage from the
correct server.

## Premise

In Internet, Inc., you play as a character who works at a
company servicing these DNS and webpage requests. You start at
the web server level responding to requests such as "Can I get
the homepage?"

** NOTE: The web server level is not currently implemented **

As you do well in your job, you get promoted to higher offices
to service different DNS requests. The offices include the
following:
- District (equivalent of \*.google.com)
- Regional (equivalent of \*.\*.com)
- Corporate (equivalent of \*.\*.\* -- Root DNS server)

This is backwards from the actual lookup process, but it follows
the idea of an employee rising through the ranks of the company.

## Gameplay

The game is currently a sorting game, where you click-and-drag
DNS requests to the appropriate boxes to be forwarded or drag
the appropriate web pages to the requestor. As you complete more
requests, the progress bar gradually fills from 0% to 100%. Once
you reach a given threshold (~90%), you will be offered a
promotion to the next office. At that point, you may choose to
proceed or stay and practice a little longer and proceed later.

## Ending the Game

Once you've completed the Root DNS Server level, you'll be
offered an option to "expand your company", but that has not yet
been implemented. Instead, it immediately goes to the "thanks
for playing" ending scene. From there, you have the option to
go to the menu or quit. Other options may be added in the future.