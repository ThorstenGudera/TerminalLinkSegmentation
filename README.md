# TerminalLinkSegmentation

.net 8.0 version of https://github.com/ThorstenGudera/GrabCutALikeEasy

GrabCut revisited: Just grab it, dont cut it.

The well known GrabCut algorithm is known to be reliable for segmenting images, but needs a lot of resources, both Memory and CPU-time. And: when using a MinCut algorithm being written from scratch and not coming from a specialized library, it may turn out, that the time of running the process will be [very] long. So, how to speed things up and lower the resources needed, by getting almost the same results?

The answer is simple: You dont need a MinCut and so dont need the whole Graph and the ResidualGraph, and, you also dont need to compute the BetaSmootheness function. This means: You can tweak the results you get from the GMMs for the terminal node capacities of the unknown pixels to get a ready to use partition of the image. We add, multiply, shift, typecast and, the most significant method, threshold the values.

So start by defining a rectangle containing the portion or the image to segment out. Classify the pixels in Background, Foreground, probably Background and probably Foreground states by setting up a mask. Init the GaussinaMixtureModels, one for the Background, one for the Foreground. Get the Probabilities from the GMMs for the unkonwn pixels as penalties (crossed over: Background GMM for the Foreground pixels and vice versa, as usual). Take the negative Log as usual to get a set of Capacities for the terminal links of the unknown area. Now threshold these results (and do other computations on it, if needed). Get the Capacities for the known pixels. Use the data we now have to partition the image.

Thats all.

A lot of example code was used writing this demo app, so e.g.:

grabcutmaster: https://github.com/moetayuko/GrabCut/tree/master

geeksforgeeks push rel: https://www.geeksforgeeks.org/fifo-push-relabel-algorithm/

paper for bk: https://discovery.ucl.ac.uk/id/eprint/13383/1/13383.pdf

closed form matting: https://github.com/MarcoForte/closed-form-matting

I maybe havent implemented the BoykovKolmogorov Algorithm correctly. I did it from the PseudoCode in the paper, but mine sometimes doesnt stop. The queue doesnt get empty, so I aded a Check-Method that compares the current path to the last checked path and breaks when similarity exceeds a given amount.

Note: Writes cached files to: LocalApplicationData\Thorsten_Gudera...

Usage:

Start the app (WinForms, .net 8.0) and open an Image, click the go-button to open the Segmentation form.
Draw a rectangle with the mouse, set the parameters like the threshold-value and click onto the Go-button. Sometimes its good to lower the number of components to return, try a value of 1. [Optional]
On the right pane, check the draw on result checkbox and draw with the mouse onto regions that should be the type you selected from the combobox (eg. Background, Foregound etc)
Click onto the Go-button again
Process the outline

Some usage examples are shown in Usage_Examples.html

Or: Check the useScribbles checkbox and the Unknown radiobutton, draw (follow) the outline of the desired object until the curve is closed. Fill the inner of it by Contextmenu as Foreground and click the create a Matte button, let the form create the trimap by button click and the click onto go.

You also could do a test for a complete result by clicking onto the DoAll button. You can draw onto the result and click that button again, if you like to edit the results.

If you implement this in a performance optimized technology and language, you could get the segmentation [almost] in real-time.

I'll look, if I can set up a mathematical model for this, which may give us a way to estimate the ideal parameter (e.g. threshold) value(s).

Disclaimer:

The code is free for academic/research purpose. Use at your own risk and we are not responsible for any loss resulting from this code.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


Info: I removed the usage_guide from the project, since the filesize simply gets too big. You can download the usage_guide from here:

https://1drv.ms/u/c/d5e5bd21dbf5e4e9/ESvgrQptooFFtpqwyufU7tEBBGs8MuHHhuCKlk1W6qb2uw?e=aUf89Z

(onedrive, link shared with anyone, no log in needed - at least thats what in the create_link_dialog was stated. If you get to a page with a big unauthenticated in the middle, you still can download the file from the button at left_top of the page.
Ok, I sometimes get a "something didn't work" error page, when clicking the link. 
If you do so either, here's what helps me in this situation. Simply click the "back" button in your browser.
After a phase of flashing and blinking, the correct file should be displayed. At least this works for me.
This really should be fixed, its exactly the link they provided [that] I added to the readme file.)

# Update:
Since complexity has risen a lot, I added a lightweight demo for better code readability (winexe project "DemoLightWeight").
