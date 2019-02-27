﻿/*
 * Your rights to use code governed by this license http://o-s-a.net/doc/license_simple_engine.pdf
 *Ваши права на использование кода регулируются данной лицензией http://o-s-a.net/doc/license_simple_engine.pdf
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OsEngine.Entity;

namespace OsEngine.Charts.CandleChart.Indicators
{
    /// <summary>
    /// Parabolic SAR
    /// </summary>
    public class ParabolicSaR : IIndicatorCandle
    {

        /// <summary>
        /// constructor with parameters. Indicator will be saved
        /// конструктор с параметрами. Индикатор будет сохраняться
        /// </summary>
        /// <param name="uniqName">unique name/уникальное имя</param>
        /// <param name="canDelete">whether user can remove indicator from chart manually/можно ли пользователю удалить индикатор с графика вручную</param>
        public ParabolicSaR(string uniqName, bool canDelete)
        {
            Name = uniqName;
            TypeIndicator = IndicatorOneCandleChartType.Line;
            ColorBase = Color.DeepSkyBlue;
            Af = 0.02;
            MaxAf = 0.2;
            PaintOn = true;
            CanDelete = canDelete;
            Load();
        }

        /// <summary>
        /// constructor without parameters.Indicator will not saved/конструктор без параметров. Индикатор не будет сохраняться
        /// used ONLY to create composite indicators/используется ТОЛЬКО для создания составных индикаторов
        /// Don't use it from robot creation layer/не используйте его из слоя создания роботов!
        /// </summary>
        /// <param name="canDelete">whether user can remove indicator from chart manually/можно ли пользователю удалить индикатор с графика вручную</param>
        public ParabolicSaR(bool canDelete)
        {
            Name = Guid.NewGuid().ToString();
            TypeIndicator = IndicatorOneCandleChartType.Line;
            ColorBase = Color.DeepSkyBlue;
            Af = 0.02;
            MaxAf = 0.2;
            PaintOn = true;
            CanDelete = canDelete;
        }

        /// <summary>
        /// all indicator values
        /// все значения индикатора
        /// </summary>
        List<List<decimal>> IIndicatorCandle.ValuesToChart
        {
            get
            {
                List<List<decimal>> list = new List<List<decimal>>();
                list.Add(Values);
                return list;
            }
        }

        /// <summary>
        /// indicator colors
        /// цвета для индикатора
        /// </summary>
        List<Color> IIndicatorCandle.Colors
        {
            get
            {
                List<Color> colors = new List<Color>();
                colors.Add(ColorBase);
                return colors;
            }

        }

        /// <summary>
        /// whether indicator can be removed from chart. This is necessary so that robots can't be removed /можно ли удалить индикатор с графика. Это нужно для того чтобы у роботов нельзя было удалить 
        /// indicators he needs in trading/индикаторы которые ему нужны в торговле
        /// </summary>
        public bool CanDelete { get; set; }

        /// <summary>
        /// indicator drawing type
        /// тип прорисовки индикатора
        /// </summary>
        public IndicatorOneCandleChartType TypeIndicator { get; set; }

        /// <summary>
        /// name of data series on which indicator will be drawn
        /// имя серии данных на которой индикатор прорисовывается
        /// </summary>
        public string NameSeries { get; set; }

        /// <summary>
        /// name of data area where indicator will be drawn
        /// имя области данных на которой индикатор прорисовывается
        /// </summary>
        public string NameArea { get; set; }

        /// <summary>
        /// value
        /// значение 
        /// </summary>
        public List<decimal> Values { get; set; }

        /// <summary>
        /// unique indicator name
        /// уникальное имя индикатора
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// indicator calculation length
        /// длинна рассчёта индикатора
        /// </summary>
        public int Lenght { get; set; }

        /// <summary>
        /// coefficient increments
        /// коэф. приращения
        /// </summary>
        public double Af { get; set; }

        /// <summary>
        /// maximum incremental value
        /// максимальный коэф. приращения
        /// </summary>
        public double MaxAf { get; set; }

        /// <summary>
        /// color of indicator line
        /// цвет линии индикатора
        /// </summary>
        public Color ColorBase { get; set; }

        /// <summary>
        /// candles to calculate indicator
        /// включена ли прорисовка серии на чарте
        /// </summary>
        public bool PaintOn { get; set; }

        /// <summary>
        /// upload settings from file
        /// загрузить настройки из файла
        /// </summary>
        public void Load()
        {
            if (!File.Exists(@"Engine\" + Name + @".txt"))
            {
                return;
            }
            try
            {

                using (StreamReader reader = new StreamReader(@"Engine\" + Name + @".txt"))
                {
                    ColorBase = Color.FromArgb(Convert.ToInt32(reader.ReadLine()));
                    Af = Convert.ToDouble(reader.ReadLine());
                    MaxAf = Convert.ToDouble(reader.ReadLine());
                    PaintOn = Convert.ToBoolean(reader.ReadLine());
                    reader.ReadLine();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                // send to log
                // отправить в лог
            }
        }

        /// <summary>
        /// save settings to file
        /// сохранить настройки в файл
        /// </summary>
        public void Save()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(@"Engine\" + Name + @".txt", false))
                {
                    writer.WriteLine(ColorBase.ToArgb());
                    writer.WriteLine(Af);
                    writer.WriteLine(MaxAf);
                    writer.WriteLine(PaintOn);
                    writer.Close();
                }
            }
            catch (Exception)
            {
                // send to log
                // отправить в лог
            }
        }

        /// <summary>
        /// delete file with settings
        /// удалить файл с настройками
        /// </summary>
        public void Delete()
        {
            if (File.Exists(@"Engine\" + Name + @".txt"))
            {
                File.Delete(@"Engine\" + Name + @".txt");
            }
        }

        /// <summary>
        /// delete data
        /// удалить данные
        /// </summary>
        public void Clear()
        {
            if (Values != null)
            {
                Values.Clear();
            }
            _myCandles = null;
        }

        /// <summary>
        /// candles used to build indicator
        /// свечи по которым строится индикатор
        /// </summary>
        private List<Candle> _myCandles;

        /// <summary>
        /// display settings window
        /// показать окно с настройками
        /// </summary>
        public void ShowDialog()
        {
            ParabolicSarUi ui = new ParabolicSarUi(this);
            ui.ShowDialog();

            if (ui.IsChange && _myCandles != null)
            {
                Reload();
            }
        }

        /// <summary>
        /// reload indicator
        /// перезагрузить индикатор
        /// </summary>
        public void Reload()
        {
            if (_myCandles == null)
            {
                return;
            }
            ProcessAll(_myCandles);


            if (NeadToReloadEvent != null)
            {
                NeadToReloadEvent(this);
            }
        }

        /// <summary>
        /// it's necessary to redraw indicator on chart
        /// необходимо перерисовать индикатор на графике
        /// </summary>
        public event Action<IIndicatorCandle> NeadToReloadEvent;

        /// <summary>
        /// additional info to calculate indicator
        /// доп. инф для расчета индикатора
        /// </summary>
        public List<decimal> MasTrend { get; set; }
        public List<decimal> MasHp { get; set; }
        public List<decimal> MasLp { get; set; }
        public List<decimal> MasAf { get; set; }
        public List<decimal> psar { get; set; }

        /// <summary>
        /// to upload new candles
        /// прогрузить новыми свечками
        /// </summary>        
        public void Process(List<Candle> candles)
        {
            if (candles == null)
            {
                return;
            }
            _myCandles = candles;
            if (Values != null && Values.Count + 1 == candles.Count)
            {
                ProcessOne(candles);
            }
            else if (Values != null && Values.Count == candles.Count)
            {
                ProcessLast(candles);
            }
            else
            {
                ProcessAll(candles);
            }
        }

        /// <summary>
        /// load only last candle
        /// прогрузить только последнюю свечку
        /// </summary>
        private void ProcessOne(List<Candle> candles)
        {
            if (candles == null) return;

            if (MasTrend == null) MasTrend = new List<decimal>();
            if (MasHp == null) MasHp = new List<decimal>();
            if (MasLp == null) MasLp = new List<decimal>();
            if (MasAf == null) MasAf = new List<decimal>();
            if (psar == null) psar = new List<decimal>();

            if (Values == null) Values = new List<decimal>();

            decimal[] dop = new decimal[6];
            if (Values.Count == 0)
            {
                dop = GetValueParabolicSar(candles, candles.Count - 1, 0, 0, 0, 0, 0, 0);
            }
            else
            {
                dop = GetValueParabolicSar(candles, candles.Count - 1, 0, Values[Values.Count - 1], MasTrend[MasTrend.Count - 1],
                    MasHp[MasHp.Count - 1], MasLp[MasLp.Count - 1], MasAf[MasAf.Count - 1]);
            }

            Values.Add(dop[0]);
            MasTrend.Add(dop[1]);
            MasHp.Add(dop[2]);
            MasLp.Add(dop[3]);
            MasAf.Add(dop[4]);
        }

        /// <summary>
        /// to upload from the beginning
        /// прогрузить с самого начала
        /// </summary>
        private void ProcessAll(List<Candle> candles)
        {
            if (candles == null) return;

            MasTrend = new List<decimal>();
            MasHp = new List<decimal>();
            MasLp = new List<decimal>();
            MasAf = new List<decimal>();

            Values = new List<decimal>();

            for (int i = 0; i < candles.Count; i++)
            {
                decimal[] dop = new decimal[6];
                if (Values.Count == 0)
                {
                    dop = GetValueParabolicSar(candles, i, 0, 0, 0, 0, 0, 0);
                }
                else
                {
                    dop = GetValueParabolicSar(candles, i, 0, Values[Values.Count - 1], MasTrend[MasTrend.Count - 1],
                        MasHp[MasHp.Count - 1], MasLp[MasLp.Count - 1], MasAf[MasAf.Count - 1]);
                }

                Values.Add(dop[0]);
                MasTrend.Add(dop[1]);
                MasHp.Add(dop[2]);
                MasLp.Add(dop[3]);
                MasAf.Add(dop[4]);
                //Values.Add(GetValueParabolicSAR(candles, i));

            }
        }

        /// <summary>
        /// overload last value
        /// перегрузить последнее значение
        /// </summary>
        private void ProcessLast(List<Candle> candles)
        {
            if (candles == null) return;

            decimal[] dop = new decimal[6];
            if (Values.Count < 2)
            {
                dop = GetValueParabolicSar(candles, candles.Count - 1, 0, 0, 0, 0, 0, 0);
            }
            else
            {
                dop = GetValueParabolicSar(candles, candles.Count - 1, 0, Values[Values.Count - 2], MasTrend[MasTrend.Count - 2],
                    MasHp[MasHp.Count - 2], MasLp[MasLp.Count - 2], MasAf[MasAf.Count - 2]);
            }

            Values[Values.Count - 1] = dop[0];
            MasTrend[MasTrend.Count - 1] = dop[1];
            MasHp[MasHp.Count - 1] = dop[2];
            MasLp[MasLp.Count - 1] = dop[3];
            MasAf[MasAf.Count - 1] = dop[4];
        }

        /// <summary>
        /// take indicator value by index
        /// взять значение индикаторм по индексу
        /// </summary>
        private decimal[] GetValueParabolicSar(List<Candle> candles, int index, int update, decimal lineP, decimal trendP, decimal hpP, decimal lpP, decimal afP)
        {
            decimal[] dop = new decimal[6];

            if (index - 2 < 1)
            {
                dop[0] = candles[index].Close;
                dop[1] = 1.0m;
                dop[2] = candles[index].High;
                dop[3] = candles[index].Low;
                dop[4] = (decimal)Af;
                dop[5] = candles[index].High;
                return dop;
            }

            if (update == 0)
            {
                if (trendP == 1.0m)
                {
                    lineP = lineP + afP * (hpP - lineP);
                }
                else
                {
                    lineP = lineP + afP * (lpP - lineP);
                }
            }

            int reverseP = 0;

            if (trendP == 1.0m)
            {
                if (candles[index].Low < lineP)
                {
                    trendP = 0.0m;
                    reverseP = 1;
                    lineP = hpP;
                    lpP = candles[index].Low;
                    afP = (decimal)Af;
                }
            }
            else
            {
                if (candles[index].High > lineP)
                {
                    trendP = 1.0m;
                    reverseP = 1;
                    lineP = lpP;
                    hpP = candles[index].High;
                    afP = (decimal)Af;
                }
            }

            if (reverseP == 0)
            {
                if (trendP == 1.0m)
                {
                    if (candles[index].High > hpP)
                    {
                        hpP = candles[index].High;
                        afP = afP + (decimal)Af;
                        if (afP > (decimal)MaxAf) afP = (decimal)MaxAf;
                    }

                    if (candles[index - 1].Low < lineP)
                        lineP = candles[index - 1].Low;

                    if (candles[index - 2].Low < lineP)
                        lineP = candles[index - 2].Low;
                }
                else
                {
                    if (candles[index].Low < lpP)
                    {
                        lpP = candles[index].Low;
                        afP = afP + (decimal)Af;
                        if (afP > (decimal)MaxAf) afP = (decimal)MaxAf;
                    }

                    if (candles[index - 1].High > lineP)
                        lineP = candles[index - 1].High;

                    if (candles[index - 2].High > lineP)
                        lineP = candles[index - 2].High;
                }
            }

            dop[0] = Math.Round(lineP, 5);
            dop[1] = trendP;
            dop[2] = hpP;
            dop[3] = lpP;
            dop[4] = afP;
            return dop;

        }

    }
}
