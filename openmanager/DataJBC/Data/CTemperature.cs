// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{

    /// <summary>
    /// This class represents a temperature value. It stores the temperature in UTI units that
    /// can easily be converted into CELSIUS and FAHRENHEIT degrees using the ToCelsius() and
    /// ToFahrenheit() methods. The temperature value can also be set in CELSIUS or FAHRENHEIT by
    /// using the InCelsius() or InFahrenheit() methods.
    /// </summary>
    /// <remarks></remarks>
    public class CTemperature : ICloneable
    {

        public enum TemperatureUnit
        {
            Celsius = 0x43, //"C"
            Fahrenheit = 0x46 //"F"
        }


        private const int STEP_CELSIOUS = 5;
        private const int STEP_FAHRENHEIT = 10;

        private int m_UTI;


        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="UTItemp">The initial value in UTI units.</param>
        /// <remarks></remarks>
        public CTemperature(int UTItemp = 0)
        {
            m_UTI = UTItemp;
        }

        public CTemperature(CTemperature temp)
        {
            m_UTI = temp.m_UTI;
        }

        public dynamic Clone()
        {
            return new CTemperature(this);
        }

        /// <summary>
        /// The current temperature stored in UTI units.
        /// </summary>
        /// <value>The desired temperature</value>
        /// <returns>The current temperature</returns>
        /// <remarks></remarks>
        public int UTI
        {
            get
            {
                return m_UTI;
            }
            set
            {
                m_UTI = value;
            }
        }

        /// <summary>
        /// Indicates if the temperature is a valid value, it is, in range.
        /// </summary>
        /// <returns>A boolean indicating if the temperature is a valid value</returns>
        /// <remarks></remarks>
        public bool isValid()
        {
            return m_UTI >= (int)TemperatureLimits.MIN_TEMP & m_UTI <= (int)TemperatureLimits.MAX_TEMP_HD;
        }

        #region CELSIOUS

        /// <summary>
        /// Sets the temperature in CELSIUS units
        /// </summary>
        /// <param name="value" >The desired temperature in CELSIUS</param>
        /// <remarks></remarks>
        public void InCelsius(int value)
        {
            m_UTI = value * 9;
        }

        /// <summary>
        /// Gets the temperature in CELSIUS units
        /// </summary>
        /// <returns>The temperature in CELSIUS</returns>
        /// <remarks></remarks>
        public int ToCelsius()
        {
            return m_UTI / 9;
        }

        public static int ToCelsius(int value)
        {
            return value / 9;
        }

        /// <summary>
        /// Gets the temperature in steps of five degrees CELSIUS.
        /// </summary>
        /// <returns>The temperature in steps of five degrees CELSIUS</returns>
        /// <remarks></remarks>
        public int ToRoundCelsius()
        {
            int aux = 0;
            int modulo = 0;

            aux = m_UTI / 9; // UTI a Celsius
            modulo = aux % STEP_CELSIOUS; // módulo (resto) istep del Celsius
            aux /= STEP_CELSIOUS; // dividido por istep para redondear

            if (modulo > STEP_CELSIOUS / 2)
            {
                aux++;
            }
            aux *= STEP_CELSIOUS; // vuelve a dejarlo por istep

            return aux;
        }

        /// <summary>
        /// Sets the temperature in CELSIUS units
        /// This temperature will be used to adjust another temperature
        /// </summary>
        /// <param name="value" >The desired temperature in CELSIUS</param>
        /// <remarks></remarks>
        public void InCelsiusToAdjust(int value)
        {
            m_UTI = value * 9;
        }

        /// <summary>
        /// Gets the temperature in CELSIUS units
        /// This temperature will be used to adjust another temperature
        /// </summary>
        /// <returns>The temperature in CELSIUS</returns>
        /// <remarks></remarks>
        public int ToCelsiusToAdjust()
        {
            return m_UTI / 9;
        }

        public void StepCelsious(int steps)
        {
            int aux = ToRoundCelsius();
            InCelsius(aux + (steps * STEP_CELSIOUS));
        }

        #endregion

        #region FAHRENHEIT

        /// <summary>
        /// Sets the temperature in FAHRENHEIT units
        /// </summary>
        /// <param name="value" >The desired temperature in FAHRENHEIT</param>
        /// <remarks></remarks>
        public void InFahrenheit(int value)
        {
            m_UTI = (value - 32) * 5;
        }

        /// <summary>
        /// Gets the temperature in FAHRENHEIT units
        /// </summary>
        /// <returns>The temperature in FAHRENHEIT.</returns>
        /// <remarks></remarks>
        public int ToFahrenheit()
        {
            return (m_UTI / 5) + 32;
        }

        /// <summary>
        /// Gets the temperature in steps of ten degrees FAHRENHEIT
        /// </summary>
        /// <returns>The temperature in steps of ten degrees FAHRENHEIT</returns>
        /// <remarks></remarks>
        public int ToRoundFahrenheit()
        {
            int aux = 0;
            int modulo = 0;

            aux = m_UTI / 5;
            aux += 32; // UTI to Fahrenheit
            modulo = aux % STEP_FAHRENHEIT; // módulo (resto) istep del Fahrenheit
            aux /= STEP_FAHRENHEIT; // dividido por istep para redondear

            if (modulo >= STEP_FAHRENHEIT / 2)
            {
                aux++;
            }
            aux *= STEP_FAHRENHEIT; // vuelve a dejarlo por istep

            return aux;
        }

        /// <summary>
        /// Sets the temperature in FAHRENHEIT units
        /// This temperature will be used to adjust another temperature
        /// </summary>
        /// <param name="value" >The desired temperature in FAHRENHEIT</param>
        /// <remarks></remarks>
        public void InFahrenheitToAdjust(int value)
        {
            m_UTI = value * 5; // do not substract 32 from value
        }

        /// <summary>
        /// Gets the temperature in FAHRENHEIT units
        /// This temperature will be used to adjust another temperature
        /// </summary>
        /// <returns>The temperature in FAHRENHEIT.</returns>
        /// <remarks></remarks>
        public int ToFahrenheitToAdjust()
        {
            return m_UTI / 5; // do not add 32 to aux
        }

        public void StepFahrenheit(int steps)
        {
            int aux = ToRoundFahrenheit();
            InFahrenheit(aux + (steps * STEP_FAHRENHEIT));
        }

        #endregion

    }
}

