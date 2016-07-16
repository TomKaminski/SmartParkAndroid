using System;
using System.Net;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Java.Lang;
using Exception = System.Exception;
using Math = System.Math;

namespace SmartParkAndroid.Core
{
    public class CircularImageView : ImageView
    {
        private static readonly ScaleType SCALE_TYPE = ScaleType.CenterCrop;

        private static readonly Bitmap.Config BitmapConfig = Bitmap.Config.Argb8888;
        private static readonly int ColordrawableDimension = 2;

        private static int DEFAULT_BORDER_WIDTH = 0;
        private static int DEFAULT_BORDER_COLOR = Color.Black;
        private static int DEFAULT_FILL_COLOR = Color.Transparent;
        private static bool DEFAULT_BORDER_OVERLAY = false;

        private RectF mDrawableRect = new RectF();
        private RectF mBorderRect = new RectF();

        private Matrix mShaderMatrix = new Matrix();
        private Paint mBitmapPaint = new Paint();
        private Paint mBorderPaint = new Paint();
        private Paint mFillPaint = new Paint();

        private int mBorderColor = DEFAULT_BORDER_COLOR;
        private int mBorderWidth = DEFAULT_BORDER_WIDTH;
        private int mFillColor = DEFAULT_FILL_COLOR;

        private Bitmap mBitmap;
        private BitmapShader mBitmapShader;
        private int mBitmapWidth;
        private int mBitmapHeight;

        private float mDrawableRadius;
        private float mBorderRadius;

        private ColorFilter mColorFilter;

        private bool mReady;
        private bool mSetupPending;
        private bool mBorderOverlay;
        private bool mDisableCircularTransformation;

        public CircularImageView(Context context) : base(context)
        {
            Init();
        }

        public CircularImageView(IntPtr a, JniHandleOwnership b) : base(a, b)
        {
        }

        public CircularImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircularImageView, 0, 0);

            mBorderWidth = a.GetDimensionPixelSize(Resource.Styleable.CircularImageView_civ_border_width, DEFAULT_BORDER_WIDTH);
            mBorderColor = a.GetColor(Resource.Styleable.CircularImageView_civ_border_color, DEFAULT_BORDER_COLOR);
            mBorderOverlay = a.GetBoolean(Resource.Styleable.CircularImageView_civ_border_overlay, DEFAULT_BORDER_OVERLAY);
            mFillColor = a.GetColor(Resource.Styleable.CircularImageView_civ_fill_color, DEFAULT_FILL_COLOR);

            a.Recycle();

            Init();
        }

        public CircularImageView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircularImageView, defStyle, 0);

            mBorderWidth = a.GetDimensionPixelSize(Resource.Styleable.CircularImageView_civ_border_width, DEFAULT_BORDER_WIDTH);
            mBorderColor = a.GetColor(Resource.Styleable.CircularImageView_civ_border_color, DEFAULT_BORDER_COLOR);
            mBorderOverlay = a.GetBoolean(Resource.Styleable.CircularImageView_civ_border_overlay, DEFAULT_BORDER_OVERLAY);
            mFillColor = a.GetColor(Resource.Styleable.CircularImageView_civ_fill_color, DEFAULT_FILL_COLOR);

            a.Recycle();

            Init();
        }

        private void Init()
        {
            SetScaleType(SCALE_TYPE);
            mReady = true;

            if (mSetupPending)
            {
                Setup();
                mSetupPending = false;
            }
        }

        public override ScaleType GetScaleType()
        {
            return SCALE_TYPE;
        }


        public override void SetScaleType(ScaleType scaleType)
        {
            if (scaleType != SCALE_TYPE)
            {
                throw new IllegalArgumentException(string.Format("ScaleType %s not supported.", scaleType));
            }
        }

        public override void SetAdjustViewBounds(bool adjustViewBounds)
        {
            if (adjustViewBounds)
            {
                throw new IllegalArgumentException("adjustViewBounds not supported.");
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (mDisableCircularTransformation)
            {
                base.OnDraw(canvas);
                return;
            }

            if (mBitmap == null)
            {
                return;
            }

            if (mFillColor != Color.Transparent)
            {
                canvas.DrawCircle(mDrawableRect.CenterX(), mDrawableRect.CenterY(), mDrawableRadius, mFillPaint);
            }
            canvas.DrawCircle(mDrawableRect.CenterX(), mDrawableRect.CenterY(), mDrawableRadius, mBitmapPaint);
            if (mBorderWidth > 0)
            {
                canvas.DrawCircle(mBorderRect.CenterX(), mBorderRect.CenterY(), mBorderRadius, mBorderPaint);
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            Setup();
        }

        public override void SetPadding(int left, int top, int right, int bottom)
        {
            base.SetPadding(left, top, right, bottom);
            Setup();
        }

        public override void SetPaddingRelative(int start, int top, int end, int bottom)
        {
            base.SetPaddingRelative(start, top, end, bottom);
            Setup();
        }

        public int GetBorderColor()
        {
            return mBorderColor;
        }

        public void SetBorderColor(int borderColor)
        {
            if (borderColor == mBorderColor)
            {
                return;
            }

            mBorderColor = borderColor;
            mBorderPaint.Color = Resources.GetColor(mBorderColor);
            Invalidate();
        }

        public int getBorderWidth()
        {
            return mBorderWidth;
        }

        public void setBorderWidth(int borderWidth)
        {
            if (borderWidth == mBorderWidth)
            {
                return;
            }

            mBorderWidth = borderWidth;
            Setup();
        }

        public bool isBorderOverlay()
        {
            return mBorderOverlay;
        }

        public void setBorderOverlay(bool borderOverlay)
        {
            if (borderOverlay == mBorderOverlay)
            {
                return;
            }

            mBorderOverlay = borderOverlay;
            Setup();
        }

        public bool isDisableCircularTransformation()
        {
            return mDisableCircularTransformation;
        }

        public void setDisableCircularTransformation(bool disableCircularTransformation)
        {
            if (mDisableCircularTransformation == disableCircularTransformation)
            {
                return;
            }

            mDisableCircularTransformation = disableCircularTransformation;
            initializeBitmap();
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            base.SetImageBitmap(bm);
            initializeBitmap();
        }

        public override void SetImageDrawable(Drawable drawable)
        {
            base.SetImageDrawable(drawable);
            initializeBitmap();
        }
        public override void SetImageResource(int resId)
        {
            base.SetImageResource(resId);
            initializeBitmap();
        }

        public void SetImageURL(string uri)
        {
            var imageBitmap = GetBitmapFromUrl(uri);
            mBitmap = imageBitmap;
            Setup();
        }

        private static Bitmap GetBitmapFromUrl(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] bytes = webClient.DownloadData(url);
                if (bytes != null && bytes.Length > 0)
                {
                    return Android.Graphics.BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                }
            }
            return null;
        }

        public override void SetColorFilter(ColorFilter cf)
        {
            if (cf == mColorFilter)
            {
                return;
            }

            mColorFilter = cf;
            ApplyColorFilter();
            Invalidate();
        }

    public ColorFilter GetColorFilter()
        {
            return mColorFilter;
        }

        private void ApplyColorFilter()
        {
            if (mBitmapPaint != null)
            {
                mBitmapPaint.SetColorFilter(mColorFilter);
            }
        }

        private Bitmap GetBitmapFromDrawable(Drawable drawable)
        {
            if (drawable == null)
            {
                return null;
            }

            if (drawable is BitmapDrawable) {
                return ((BitmapDrawable)drawable).Bitmap;
            }

            try
            {
                Bitmap bitmap;

                if (drawable is ColorDrawable) {
                    bitmap = Bitmap.CreateBitmap(ColordrawableDimension, ColordrawableDimension, BitmapConfig);
                } else {
                    bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, BitmapConfig);
                }

                Canvas canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                drawable.Draw(canvas);
                return bitmap;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void initializeBitmap()
        {
            if (mDisableCircularTransformation)
            {
                mBitmap = null;
            }
            else
            {
                mBitmap = GetBitmapFromDrawable(this.Drawable);
            }
            Setup();
        }

        private void Setup()
        {
            if (!mReady)
            {
                mSetupPending = true;
                return;
            }

            if (Width == 0 && Height == 0)
            {
                return;
            }

            if (mBitmap == null)
            {
                Invalidate();
                return;
            }

            mBitmapShader = new BitmapShader(mBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

            mBitmapPaint.AntiAlias = (true);
            mBitmapPaint.SetShader(mBitmapShader);

            mBorderPaint.SetStyle(Paint.Style.Stroke);
            mBorderPaint.AntiAlias = (true);
            mBorderPaint.Color = Color.White;
            mBorderPaint.StrokeWidth = (mBorderWidth);

            mFillPaint.SetStyle(Paint.Style.Fill);
            mFillPaint.AntiAlias = (true);
            mFillPaint.Color = Color.White;

            mBitmapHeight = mBitmap.Height;
            mBitmapWidth = mBitmap.Width;

            mBorderRect.Set(CalculateBounds());
            mBorderRadius = Math.Min((mBorderRect.Height() - mBorderWidth) / 2.0f, (mBorderRect.Width() - mBorderWidth) / 2.0f);

            mDrawableRect.Set(mBorderRect);
            if (!mBorderOverlay && mBorderWidth > 0)
            {
                mDrawableRect.Inset(mBorderWidth - 1.0f, mBorderWidth - 1.0f);
            }
            mDrawableRadius = Math.Min(mDrawableRect.Height() / 2.0f, mDrawableRect.Width() / 2.0f);

            ApplyColorFilter();
            UpdateShaderMatrix();
            Invalidate();
        }

        private RectF CalculateBounds()
        {
            int availableWidth = Width - PaddingLeft - PaddingRight;
            int availableHeight = Height - PaddingTop - PaddingBottom;

            int sideLength = Math.Min(availableWidth, availableHeight);

            float left = PaddingLeft + (availableWidth - sideLength) / 2f;
            float top = PaddingTop + (availableHeight - sideLength) / 2f;

            return new RectF(left, top, left + sideLength, top + sideLength);
        }

        private void UpdateShaderMatrix()
        {
            float scale;
            float dx = 0;
            float dy = 0;

            mShaderMatrix.Set(null);

            if (mBitmapWidth * mDrawableRect.Height() > mDrawableRect.Width() * mBitmapHeight)
            {
                scale = mDrawableRect.Height() / mBitmapHeight;
                dx = (mDrawableRect.Width() - mBitmapWidth * scale) * 0.5f;
            }
            else
            {
                scale = mDrawableRect.Width() / mBitmapWidth;
                dy = (mDrawableRect.Height() - mBitmapHeight * scale) * 0.5f;
            }

            mShaderMatrix.SetScale(scale, scale);
            mShaderMatrix.PostTranslate((int)(dx + 0.5f) + mDrawableRect.Left, (int)(dy + 0.5f) + mDrawableRect.Top);

            mBitmapShader.SetLocalMatrix(mShaderMatrix);
        }

    }
}