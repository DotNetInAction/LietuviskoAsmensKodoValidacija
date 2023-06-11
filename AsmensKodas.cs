using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asmkodas
{
    /// <summary>
    /// Pakankamai aiškus enumeratorius.
    /// </summary>
    public enum Lytis { Vyras, Moteris };

    /// <summary>
    /// Klasė validuojanti asmens kodą
    /// </summary>
    public class AsmensKodas
    {
        private string _asmensKodas = string.Empty;

        private DateTime _gimimoData;
        /// <summary>
        /// Asmens gimimo data nustatyta pagal asmens kodą.
        /// </summary>
        public DateTime GimimoData
        {
            get
            {
                return _gimimoData;
            }
        }

        private Lytis _lytis;
        /// <summary>
        /// Lytis
        /// </summary>
        public Lytis Lytis
        {
            get
            {
                return _lytis;
            }
        }

        private bool _isValid = false;
        /// <summary>
        /// Ar asmens kodas sudarytas teisingai
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }


        public AsmensKodas(string _asmensKodas)
        {
            if (_asmensKodas == null)
                throw new ArgumentNullException("_asmensKodas");

            this._asmensKodas = _asmensKodas;

            //Validacijos metodas. Čia bus aprašoma visa validacijos logika.
            Validate();
        }

        private void Validate()
        {            
            int lytis;
            int metai;
            int menuo;
            int diena;

            try
            {
                //Asmens kodo ilgis turi būti 11 skaitmenų ilgio.
                if (_asmensKodas.Length != 11)
                    _isValid = false;

                //Nusistatome lyti. Jei pirmas skaitmuo lyginis - moteris, jei ne - vyras.
                int.TryParse(_asmensKodas[0].ToString(), out lytis);
                _lytis = ((lytis % 2) == 0) ? Lytis.Moteris : Lytis.Vyras;

                //2-7 Skaičiai yra gimimo data. Bandome nusiskaityti ir sudaryti DateTime objektą.
                string tempData = _asmensKodas.Substring(1, 6);

                //Ištraukiame metus 2-3 skaičiai
                int.TryParse(tempData.Substring(0, 2), out metai);
                //Ištraukiame mėnesį 4-5 skaičiai
                int.TryParse(tempData.Substring(2, 2), out menuo);
                //Ištraukiame dieną 6-7 skaičiai
                int.TryParse(tempData.Substring(4, 2), out diena);

                //Nusistame šimtmetį. Kadangi metai pateikiami sutrumpintame formate, mums reikia
                //žinoti iš kurio amžiaus yra asmuo. Tai sužinome iš pirmo asmens kodo 
                //skaičiaus. Žmonėms gimusiems 20 amžiuje suteikiami skaičiai 3-4, gimusiems 21 amžiuje
                //5-6 ir t.t.. 
                int simtmetis = 0;
                switch (lytis)
                {
                    case 1:
                    case 2:
                        simtmetis = 1800;
                        break;
                    case 3:
                    case 4:
                        simtmetis = 1900;
                        break;
                    case 5:
                    case 6:
                        simtmetis = 2000;
                        break;
                    default:
                        simtmetis = 1900;
                        break;
                }

                //Sudedame metus iš asmens kodo ir nustatytą amžių. Gauname tikrus metus.
                metai = simtmetis + metai;

                //Sudarome DateTime objektą ir galėsime surasti ją savybėje GimimoData.
                _gimimoData = new DateTime(metai, menuo, diena);

                //Asmens kodą pasiverčiame į INT masyvą. Taip bus patogiau dirbti.
                int[] asmensKodasArray = new int[11];
                for (int i = 0; i < 11; i++)
                {
                    asmensKodasArray[i] = int.Parse(_asmensKodas.Substring(i, 1));
                }

                //Kintamajame laikysime kontrolinės sumos rezultatą.
                int kontrolineSuma = 0;

                //Skaičiuojame asmens kodo kontrolinę sumą pagal pirmą variantą.
                //L*1 + Y*2 + Y*3 + M*4 + M*5 + D*6 + D*7 + X*8 + X*9 + X*1
                for (int i = 1; i <= 10; i++)
                {
                    if (i < 10)
                        kontrolineSuma += asmensKodasArray[i - 1] * i;
                    else
                        kontrolineSuma += asmensKodasArray[i - 1] * 1; //Suprantu kad šis žingsnis nėra logiškas, tačiau puikiai iliustruoja algoritmą.
                }

                //Bandome pirmą sumą. Daliname sumą iš simbolių kiekio asmens kode
                int kontrolinesSumosRezultatas = kontrolineSuma % _asmensKodas.Length;

                //Jei gauta kontrolinis skaičius yra didesnis už 9, vadinasi mums reikalinga antroji suma.
                //Kitu atveju rezultatas yra tas kurio mums reikia.
                //suma = X1*3 + X2*4 + X3*5 + X4*6 + X5*7 + X6*8 + X7*9 + X8*1 + X9*2 + X10*3
                if (kontrolinesSumosRezultatas > 9)
                {
                    //Atstatome kontrolinę sumą į pradinę padėtį
                    kontrolineSuma = 0;
                    for (int i = 1; i <= 10; i++)
                    {
                        if (i < 8)
                            kontrolineSuma += (asmensKodasArray[i - 1] * (i + 2));
                        else
                            kontrolineSuma += (asmensKodasArray[i - 1] * (i - 7));
                    }
                    kontrolinesSumosRezultatas = kontrolineSuma % _asmensKodas.Length;
                }

                //Jei gautas skaičius didesnis už 9, prilyginame jį nuliui. 
                //Tai darome tam jog kontrolinę sumą lyginsime su kontroliniu skaičiumi
                //kadangi kontrolinis skaičius yra paskutinis asmens kodo skaičius, reiškia kontrolinė suma 
                //negali būti didesnė už 9.
                if (kontrolinesSumosRezultatas > 9)
                    kontrolinesSumosRezultatas = 0;

                //Kontrolinis skaičius - paskutinis asmens kodo skaičius.
                int kontrolinisSkaicius = int.Parse(_asmensKodas.Substring(10, 1));

                //Kontrolinis skaičius turi būti lygus mūsų gautam skaičiui. 
                //Tai ir yra pagrindinė validacija
                if (kontrolinisSkaicius != kontrolinesSumosRezultatas)
                    _isValid = false;
                else
                    _isValid = true;
            }
            //Bet kokios nenumatytos klaidos atveju 
            //sakysime jog asmens kodas neteisingas.
            catch 
            {
                _isValid = false;
            }
        }
    }
}
