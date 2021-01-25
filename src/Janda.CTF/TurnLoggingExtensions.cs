using Microsoft.Extensions.Logging;
using RightTurn;
using System;

namespace Janda.CTF
{
    internal static class TurnLoggingExtensions
    {
        public static ITurn WithLogging(this ITurn turn, Action<ILoggingBuilder, ITurn> loggingWithTurn)
        {
            turn.Directions.Add<Action<ILoggingBuilder>>((logging) => loggingWithTurn.Invoke(logging, turn));
            return turn;
        }
    }
}
