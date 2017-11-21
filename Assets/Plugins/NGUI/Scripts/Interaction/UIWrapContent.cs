//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This script makes it possible for a scroll view to wrap its content, creating endless scroll views.
/// Usage: simply attach this script underneath your scroll view where you would normally place a UIGrid:
/// 
/// + Scroll View (with panel)
/// |- UIWrappedContent
/// |-- Item 1
/// |-- Item 2
/// |-- Item 3
/// 
/// 摆放prefab注意点：
/// 1）通过Anchor来实现不同分辨率下Panel区域大小适配
/// 2) 初始Item数量要保证在最大可能分辨率下能布满整个UIPanel区域，再额外多3个Item，否则Item不能复用
/// 使用方式：
/// 1）InitChildren：初始化孩子列表，指定要显示的孩子个数，多余的孩子会被隐藏
/// 2）RestToBeginning：初始化列表位置，使复位到最顶部或者最左端
/// 3）onInitializeItem：设置回调，刷新Item
/// 注意：使用UIDragDropItem时，UIWrapContent不能同UIGrid或者UITable共存
/// </summary>

[AddComponentMenu("NGUI/Interaction/Wrap Content")]
public class UIWrapContent : MonoBehaviour
{
    public delegate void OnInitializeItem (GameObject go, int wrapIndex, int realIndex);
    
    /// <summary>
	/// Width of the child items for positioning purposes.
	/// </summary>

    public int itemWidth = 100;

    /// <summary>
    /// height of the child items for positioning purposes.
    /// </summary>

    public int itemHeight = 100;


	/// <summary>
	/// Whether the content will be automatically culled. Enabling this will improve performance in scroll views that contain a lot of items.
	/// </summary>

	public bool cullContent = true;

	/// <summary>
	/// Minimum allowed index for items. If "min" is equal to "max" then there is no limit.
	/// For vertical scroll views indices increment with the Y position (towards top of the screen).
	/// </summary>

	public int minIndex = 0;

	/// <summary>
	/// Maximum allowed index for items. If "min" is equal to "max" then there is no limit.
	/// For vertical scroll views indices increment with the Y position (towards top of the screen).
	/// </summary>

	public int maxIndex = 0;

	/// <summary>
	/// Callback that will be called every time an item needs to have its content updated.
	/// The 'wrapIndex' is the index within the child list, and 'realIndex' is the index using position logic.
	/// </summary>

	public OnInitializeItem onInitializeItem;

    /// <summary>
    /// Maximum columns per line. <= 0 is unlimited
    /// </summary>

    public int maxPerLine = 0;

    /// <summary>
    /// left arrow when Horizontal or bottom arrow when Vertical
    /// </summary>

    public GameObject arrow1;

    /// <summary>
    /// right arrow when Horizontal or bottom arrow when Vertical
    /// </summary>

    public GameObject arrow2;

    public bool isDebug = false;

    Transform mTrans;
	UIPanel mPanel;
	UIScrollView mScroll;
    bool mHorizontal = false;
    bool mForceReset = true;
	List<Transform> mChildren = new List<Transform>();

    bool mCachedScrollView = false;
    Vector2 mOriginalClipOffset = Vector2.zero;
    Vector3 mOriginalPosition = Vector3.zero;

    bool mUpdateAnchors = false;
    int mUpdateFrame = 0;
    protected bool mNeedToWrap = false;
    
	/// <summary>
	/// Initialize everything and register a callback with the UIPanel to be notified when the clipping region moves.
	/// </summary>

	protected virtual void Start ()
    {
        if (!CacheScrollView())
        {
            this.enabled = false;
        }
        else
        {
#if UNITY_EDITOR
            if (!Application.isPlaying || isDebug)
            {
                RestToBeginning();
            }
#endif
        }
    }

    /// <summary>
    /// Callback triggered by the UIPanel when its clipping region moves (for example when it's being scrolled).
    /// </summary>

    protected virtual void OnMove(UIPanel panel)
    {
        WrapContent();
        CheckArrows();
    }

    void CheckArrows()
    {
        if (!CacheScrollView())
        {
            return;
        }

        if (arrow1)
        {
            arrow1.SetActive(!IsVisible(0));
        }

        if (arrow2)
        {
            arrow2.SetActive(!IsVisible(maxIndex));
        }
    }

    /// <summary>
    /// Cache the scroll view and return 'false' if the scroll view is not found.
    /// </summary>

    protected bool CacheScrollView()
    {
        if (mCachedScrollView)
        {
            return mScroll != null && (mScroll.movement == UIScrollView.Movement.Horizontal || mScroll.movement == UIScrollView.Movement.Vertical);
        }

        mCachedScrollView = true;
        mTrans = transform;
        mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        if (mPanel == null) return false;
        mOriginalClipOffset = mPanel.clipOffset;
        mOriginalPosition = mPanel.transform.localPosition;

        mScroll = mPanel.GetComponent<UIScrollView>();
        if (mScroll == null) return false;
        if (mScroll.movement == UIScrollView.Movement.Horizontal) mHorizontal = true;
        else if (mScroll.movement == UIScrollView.Movement.Vertical) mHorizontal = false;
        else return false;
        return true;
    }

    public virtual void InitChildren(List<Transform> children, int showCount = -1)
    {
        if (!CacheScrollView())
        {
            return;
        }

        if (children != null)
        {
            minIndex = 0;
            maxIndex = showCount - 1;
            mNeedToWrap = showCount > children.Count;

            mChildren.Clear();
            if (showCount < 0 || showCount > children.Count)
            {
                showCount = children.Count;
            }
            for (int i = 0; i < children.Count; i++)
            {
                if (showCount > 0 && children[i] != null)
                {
                    children[i].gameObject.SetActive(true);
                    mChildren.Add(children[i]);
                    showCount--;
                }
                else if(children[i] != null)
                {
                    children[i].gameObject.SetActive(false);
                }
            }
        }
    }

    protected virtual void ResetPanel()
    {
        if (mPanel)
        {
            // must close callback at first
            mPanel.onClipMove -= OnMove;
            mPanel.transform.localPosition = mOriginalPosition;
            mPanel.clipOffset = mOriginalClipOffset;
            mPanel.onClipMove += OnMove;
        }
    }

    protected void ResetScrollView()
    {
        if (mScroll != null)
        {
            // must close callback at first
            if (mPanel) mPanel.onClipMove -= OnMove;
            mScroll.ResetPosition();
            if (mPanel) mPanel.onClipMove += OnMove;
        }
    }

	/// <summary>
	/// Helper function that resets the position of all the children.
	/// </summary>

	void ResetChildPositions ()
	{
#if UNITY_EDITOR
        if(!Application.isPlaying || isDebug)
        {
            mChildren.Clear();
            for (int i = 0; i < mTrans.childCount; ++i)
                mChildren.Add(mTrans.GetChild(i));
        }
#endif
        for (int i = 0, imax = mChildren.Count; i < imax; ++i)
		{
			Transform t = mChildren[i];
#if UNITY_EDITOR
            if (!Application.isPlaying || isDebug)
            {
                t.name = i.ToString();
            }
#endif
            t.localPosition = GetLocalPosition(i);
        }
    }

    protected Vector3 GetLocalPosition(int realIndex)
    {
        if (mHorizontal)
        {
            return maxPerLine > 0 ? new Vector3((realIndex / maxPerLine) * itemWidth, -(realIndex % maxPerLine) * itemHeight, 0f) :
                new Vector3(realIndex * itemWidth, 0f, 0f);
        }
        else
        {
            return maxPerLine > 0 ? new Vector3((realIndex % maxPerLine) * itemWidth, -(realIndex / maxPerLine) * itemHeight, 0f) :
                new Vector3(0f, -realIndex * itemHeight, 0f);
        }
    }

    protected float GetExtents()
    {
        if (mHorizontal)
        {
            return maxPerLine > 0 ? itemWidth * (((mChildren.Count - 1) / maxPerLine) + 1) : itemWidth * mChildren.Count;
        }
        else
        {
            return maxPerLine > 0 ? itemHeight * (((mChildren.Count - 1) / maxPerLine) + 1) : itemHeight * mChildren.Count;
        }
    }

    public bool IsVisible(int realIndex)
    {
        Vector3 loaclPosition = GetLocalPosition(realIndex);
        Vector3[] corners = mPanel.worldCorners;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }

        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
        float extents = GetExtents();
        if (mHorizontal)
        {
            float distance = loaclPosition.x - center.x;
            if (extents > mPanel.finalClipRegion.z)
            {
                return Mathf.Abs(distance) < ((mPanel.finalClipRegion.z + itemWidth) * 0.5f - mPanel.clipSoftness.x);
            }
            else
            {
                return true;
            }
        }
        else
        {
            float distance = loaclPosition.y - center.y;
            if (extents > mPanel.finalClipRegion.w)
            {
                return Mathf.Abs(distance) < ((mPanel.finalClipRegion.w + itemHeight) * 0.5f - mPanel.clipSoftness.y);
            }
            else
            {
                return true;
            }
        }
    }

    [ContextMenu("Rest To Beginning")]
    public virtual void RestToBeginning()
    {
        if (!CacheScrollView())
        {
            return;
        }

        ResetPanel();
        ResetChildPositions();
        ResetScrollView();
        // force update
        mForceReset = true;
        WrapContent();
        mForceReset = false;
        CheckArrows();
    }

    public void UpdateAnchors()
    {
        mUpdateAnchors = true;
        mUpdateFrame = Time.frameCount;
    }

    void LateUpdate()
    {
        // wait one frame
        if (mUpdateAnchors && Time.frameCount > mUpdateFrame)
        {
            ResetScrollView();
            mUpdateAnchors = false;
        }
    }

    /// <summary>
    /// Wrap all content, repositioning all children as needed.
    /// </summary>

    public void WrapContent()
    {
        if (!CacheScrollView())
        {
            return;
        }

        bool allWithinRange = true;
        if (mNeedToWrap || mForceReset)
        {
            Vector3[] corners = mPanel.worldCorners;
            for (int i = 0; i < 4; ++i)
            {
                Vector3 v = corners[i];
                v = mTrans.InverseTransformPoint(v);
                corners[i] = v;
            }

            if (mHorizontal)
            {
                allWithinRange = WrapHorizontal(corners);
            }
            else
            {
                allWithinRange = WrapVertical(corners);
            }

            if (mForceReset && !mNeedToWrap)
            {
                allWithinRange = false;
            }
        }
        else
        {
            allWithinRange = false;
        }
        mScroll.restrictWithinPanel = !allWithinRange;
    }

    protected int GetRealIndex(Vector3 localPosition)
    {
        if (mHorizontal)
        {
            return maxPerLine > 0 ? Mathf.RoundToInt(localPosition.x / itemWidth) * maxPerLine + Mathf.RoundToInt(-localPosition.y / itemHeight) :
                Mathf.RoundToInt(localPosition.x / itemWidth);
        }
        else
        {
            return maxPerLine > 0 ? Mathf.RoundToInt(-localPosition.y / itemHeight) * maxPerLine + Mathf.RoundToInt(localPosition.x / itemWidth) :
                Mathf.RoundToInt(-localPosition.y / itemHeight);
        }
    }

    protected bool WrapHorizontal(Vector3[] corners)
    {
        float extents = GetExtents();
        bool allWithinRange = true;
        float halfExtents = extents * 0.5f;
        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);

        for (int i = 0, imax = mChildren.Count; i < imax; ++i)
        {
            Transform t = mChildren[i];
            float distance = t.localPosition.x - center.x;
            
            if (mForceReset)
            {
                // update all
                int realIndex = GetRealIndex(t.localPosition);
                if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                {
                    t.name = realIndex.ToString();
                    UpdateItem(t, i, realIndex);
                }
                if (extents <= mPanel.finalClipRegion.z)
                {
                    allWithinRange = false;
                } 
            }
            else if (extents > mPanel.finalClipRegion.z)
            {
                // if out of panel, update
                if (Mathf.Abs(distance) > halfExtents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x = distance < 0f ? pos.x + extents : pos.x - extents;
                    distance = pos.x - center.x;
                    int realIndex = GetRealIndex(pos);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        t.name = realIndex.ToString();
                        UpdateItem(t, i, realIndex);
                    }
                    else
                    {
                        allWithinRange = false;
                    }
                }
            }
            else
            {
                allWithinRange = false;
            }
        }
        if (cullContent && mNeedToWrap)
        {
            float min = corners[0].x - itemWidth;
            float max = corners[2].x + itemWidth;

            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                if (!UICamera.IsPressed(mChildren[i].gameObject))
                {
                    bool active = allWithinRange ? mChildren[i].localPosition.x > min && mChildren[i].localPosition.x < max : true;
                    NGUITools.SetActive(mChildren[i].gameObject, active, false);
                }
            }
        }
        return allWithinRange;
    }

    protected bool WrapVertical(Vector3[] corners)
    {
        float extents = GetExtents();
        bool allWithinRange = true;
        float halfExtents = extents * 0.5f;
        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);

        for (int i = 0, imax = mChildren.Count; i < imax; ++i)
        {
            Transform t = mChildren[i];
            float distance = t.localPosition.y - center.y;

            if (mForceReset)
            {
                // update all
                int realIndex = GetRealIndex(t.localPosition);
                if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                {
                    t.name = realIndex.ToString();
                    UpdateItem(t, i, realIndex);
                }
                if (extents <= mPanel.finalClipRegion.w)
                { 
                    allWithinRange = false;
                } 
            }
            else if (extents > mPanel.finalClipRegion.w)
            {
                // if out of panel, update
                if (Mathf.Abs(distance) > halfExtents)
                {
                    Vector3 pos = t.localPosition;
                    pos.y = distance < 0f ? pos.y + extents : pos.y - extents;
                    distance = pos.y - center.y;
                    int realIndex = GetRealIndex(pos);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        t.name = realIndex.ToString();
                        UpdateItem(t, i, realIndex);
                    }
                    else
                    {
                        allWithinRange = false;
                    }
                }
            }
            else
            {
                allWithinRange = false;
            }
        }
        if (cullContent && mNeedToWrap)
        {
            float min = corners[0].y - itemHeight;
            float max = corners[2].y + itemHeight;

            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                if(!UICamera.IsPressed(mChildren[i].gameObject))
                {
                    bool active = allWithinRange ? mChildren[i].localPosition.y > min && mChildren[i].localPosition.y < max : true;
                    NGUITools.SetActive(mChildren[i].gameObject, active, false);
                }
            }
        }
        return allWithinRange;
    }

	/// <summary>
	/// Sanity checks.
	/// </summary>

	void OnValidate ()
	{
        if (maxIndex < minIndex)
        {
            maxIndex = minIndex;
        }
        if (minIndex > maxIndex)
        {
            maxIndex = minIndex;
        }
        if (itemWidth <= 0)
        {
            itemWidth = 1;
        }
        if (itemHeight <= 0)
        {
            itemHeight = 1;
        }
    }

    void OnDestroy()
    {
        if (mPanel != null)
        {
            mPanel.onClipMove -= OnMove;
        }
    }

	/// <summary>
	/// Want to update the content of items as they are scrolled? Override this function.
	/// </summary>

	protected virtual void UpdateItem (Transform item, int index, int realIndex)
	{
		if (onInitializeItem != null)
		{
			onInitializeItem(item.gameObject, index, realIndex);
		}
	}
}
