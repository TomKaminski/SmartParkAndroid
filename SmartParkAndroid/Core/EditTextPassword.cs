using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;

namespace SmartParkAndroid.Core
{
    public enum IconPosition
    {
        Left = 0,
        Right = 1
    }

    public sealed class EditTextPassword : AppCompatEditText
    {
        private int _textPassword = 129;
        private int _numberPassword = 18;

        private bool isPasswordVisible = true;
        private Drawable icon;
        private IconPosition iconPosition;
        private Typeface typeface;

        private int showPasswordIcon;
        private int hidePasswordIcon;
        private InputTypes inputType;

        public EditTextPassword(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public EditTextPassword(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            if (!IsInEditMode)
            {
                Init(attrs);
            }
        }

        public EditTextPassword(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            if (!IsInEditMode)
            {
                Init(attrs);
            }
        }

        public EditTextPassword(Context context) : base(context)
        {
        }

        private void Init(IAttributeSet attrs)
        {
            if (attrs != null)
            {
                var typedArray = Context.ObtainStyledAttributes(attrs, Resource.Styleable.EditTextPassword);
                iconPosition = IconPosition.Right;
                showPasswordIcon = typedArray.GetResourceId(Resource.Styleable.EditTextPassword_showPasswordIcon,
                    Resource.Drawable.ic_show_password);
                hidePasswordIcon = typedArray.GetResourceId(Resource.Styleable.EditTextPassword_hidePasswordIcon,
                    Resource.Drawable.ic_hide_password);
                typedArray.Recycle();
            }

            this.inputType = InputType;
            TogglePassword();

        }

        private void TogglePassword()
        {
            if (isPasswordVisible)
            {
                InputType = inputType;
            }
            else
            {
                InputType = inputType == InputTypes.TextVariationPassword ? 
                    InputTypes.TextVariationPassword : 
                    InputTypes.TextVariationVisiblePassword;
            }

            isPasswordVisible = !isPasswordVisible;
            int textLength = Text.Length;
            SetSelection(textLength, textLength);
            setupIcon();
        }


        private void setupIcon()
        {
            this.icon = this.isPasswordVisible ? GetDrawable(this.showPasswordIcon) : GetDrawable(hidePasswordIcon);
            switch (this.iconPosition)
            {
                case IconPosition.Left:
                {
                    SetCompoundDrawablesWithIntrinsicBounds(this.icon, null, null, null);
                    break;
                }
                case IconPosition.Right:
                {
                    SetCompoundDrawablesWithIntrinsicBounds(null, null, this.icon, null);
                    break;
                }
            }
            CompoundDrawablePadding = 10;
        }


        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (ev.Action == MotionEventActions.Up)
            {
                int x = (int) ev.GetX();

                switch (this.iconPosition)
                {
                    case IconPosition.Left:
                    {
                        int iconWidth = icon.Bounds.Width();
                        if (x >= PaddingLeft && x <= PaddingLeft + iconWidth)
                        {
                            TogglePassword();
                            ev.Action = MotionEventActions.Cancel;
                        }
                        break;
                    }
                    case IconPosition.Right:
                    {
                        int iconWidth = icon.Bounds.Width();
                        if (x >= (Width - PaddingLeft) - iconWidth && x <= Width + iconWidth - PaddingRight)
                        {
                            TogglePassword();
                            ev.Action = MotionEventActions.Cancel;
                        }
                        break;
                    }
                }
            }
            return base.OnTouchEvent(ev);
        }

        public int GetShowPasswordIcon()
        {
            return showPasswordIcon;
        }

        public void SetShowPasswordIcon(int showPwdIc)
        {
            showPasswordIcon = showPwdIc;
        }

        public int GetHidePasswordIcon()
        {
            return hidePasswordIcon;
        }

        public void SetHidePasswordIcon(int hidePwdIc)
        {
            hidePasswordIcon = hidePwdIc;
        }


        public bool IsPasswordVisible()
        {
            return isPasswordVisible;
        }


        private Drawable GetDrawable(int id)
        {
            return ContextCompat.GetDrawable(Context, id);
        }
    }
}