# TEAM-39

## Inspiration
The unboxing experience of an XR device involves the use of an on-screen keyboard. For many users, this will be the first time they provide complex input in XR space. First impressions are important! The general sentiment of XR keyboards are negative - they require exaggerated typing motions or precise pointing. This can be difficult for XR users new and old, especially users with limited fine motor control. We want to improve accessibility and user friendliness for everyone who wants to input text.

## What it does
We use hand tracking and gesture recognition to express keyboard input. The left hand indicates the column of the keyboard (the fingertips and columns are color-coded), and the right hand indicates the row. We also have special gestures for common actions (backspace, space, enter, etc)

## How we built it
We used Unity + MRTK library for the demonstration. We used Blender for modeling and Adobe After Effects and Premiere for creating the video.

## Challenges we ran into
Our development progress was heavily delayed by bringing up the hand-tracking on the Oculus Quest 2. It turns out will not work out of the box and many settings and dependencies are not development friendly. Since many of us were developing on new or borrowed hardware many initial obstacles such as basic setup for development was significantly underestimated. Creating custom MRTK events for hand and individual finger joint tracking took most of our development time to properly integrate into our design.

## Accomplishments that we're proud of
Getting hand-tracking working - we were luckily able to get a working demo of our idea! There is a lot of polish in our design, documentation, and showcase.

## What we learned
Materiality of design elements for our showcase, and we learned about accessibility in the ease of hand gestures - we had to figure out which motions would be easy for people to learn and use. It was also our first project that utilizes the MRTK framework.

## What's next for KeyTips
Refining the gesture recognition so that text input is more accurate. There may be other approaches to detecting pinch that will work better, or it may be a limitation of the precision of hand tracking on the Quest 2. Additionally, packaging it as a Unity Asset so that others can use it as a source of text input.

Brainstorm board: https://miro.com/app/board/uXjVOC5eDF8=/
