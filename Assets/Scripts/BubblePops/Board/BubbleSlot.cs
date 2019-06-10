using BubblePops.ScriptableObjects;

namespace BubblePops.Board
{
    public class BubbleSlot
	{
        private readonly BubbleSlotView _view;
		private BubbleConfigItem _bubbleConfig;

        public BubbleSlot(BubbleSlotView view)
		{
            _view = view;
            _view.SetBubbleSlot(this);
        }

        public void PlaceBubble(BubbleConfigItem bubbleConfig)
        {
			_bubbleConfig = bubbleConfig;
        	_view.SetBubbleColor(bubbleConfig.color);
        	_view.SetBubbleNumber(bubbleConfig.display);
        }

        public bool IsEmpty()
        {
            return !HasBubble();
        }

        public bool HasBubble()
        {
            return _bubbleConfig != null;
        }
    }


}