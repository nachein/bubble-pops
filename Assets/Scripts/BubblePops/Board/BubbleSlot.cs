using System;
using BubblePops.ScriptableObjects;

namespace BubblePops.Board
{
    public class BubbleSlot
	{
        private Guid _id;
        private readonly BubbleSlotView _view;
		private BubbleConfigItem _bubbleConfig;
        private bool _isReserved;

        public BubbleSlot(BubbleSlotView view)
		{
            _id = Guid.NewGuid();
            _view = view;
            _view.SetBubbleSlot(this);
        }

        public void PlaceBubble(BubbleConfigItem bubbleConfig)
        {
			_bubbleConfig = bubbleConfig;
        	_view.SetBubbleColor(bubbleConfig.color);
        	_view.SetBubbleNumber(bubbleConfig.display);
            _isReserved = false;
        }

        public bool IsEmpty()
        {
            return !HasBubble();
        }

        public bool HasBubble()
        {
            return _bubbleConfig != null;
        }

        public bool IsReserved()
        {
            return _isReserved;
        }

        public void Reserve()
        {
            _isReserved = true;
        }

        public BubbleConfigItem BubbleConfig()
        {
            return _bubbleConfig;
        }

        public Guid Id()
        {
            return _id;
        }

        public BubbleSlotView View()
        {
            return _view;
        }

        public void Pop()
        {
            _isReserved = false;
            _bubbleConfig = null;
            _view.Pop();
        }
    }


}
