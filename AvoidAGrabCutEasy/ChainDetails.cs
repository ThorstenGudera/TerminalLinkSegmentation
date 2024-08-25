namespace AvoidAGrabCutEasy
{
    public class ChainDetails
    {
        //this is new and will be a details container for Chain-Coords
        //to store information of what part(s) of the current ChainCode to use.
        //Background: for pictures that are hard to segment with low user action (rectangle or just a couple of bg and fg scribbles)
        //we need something to improve the results. So I added the option to draw Unknown parts (so you could follow_by_drawing
        //the outline of the object). This might be a tedious step of work, so I thought, it might be possible to get at least a part
        //of the objects outline by scanning a gradient picture of the image. That introduces the ChainCode Form, which also is in an early
        //development state.
        public int ID { get; set; }
        public int Strt { get; set; }
        public int Ed { get; set; }
        public int Cnt { get; set; }
    }
}