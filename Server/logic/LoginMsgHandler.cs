using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Online_Tank_War
{
    public partial class MsgHandler
    {
        public static void MsgRegister(ClientState clientState, ProtoBuf.IExtensible msgBase)
        {
            proto.LoginMsg.MsgRegister msgRegister = new proto.LoginMsg.MsgRegister();

            //注册成功回复客户端result = 0, 失败回复1；
            if (DataBaseManager.Register(msgRegister.id, msgRegister.pw))
            {
                DataBaseManager.CreatePlayer(msgRegister.id);
                msgRegister.result = 0;
            }
            else
            {
                msgRegister.result = 1;
            }

            NetManager.Send(clientState, msgRegister);
        }

        public static void MsgLogin(ClientState clientState, ProtoBuf.IExtensible msgBase)
        {
            proto.LoginMsg.MsgLogin msgLogin = new proto.LoginMsg.MsgLogin();

            if (!DataBaseManager.CheckPassword(msgLogin.id, msgLogin.pw))
            {
                msgLogin.result = 1;
                NetManager.Send(clientState, msgLogin);
                return;
            }

            if (clientState.player != null)
            {
                msgLogin.result = 1;
                NetManager.Send(clientState, msgLogin);
                return;
            }

            if (PlayerManager.IsOnline(msgLogin.id))
            {
                Player other = PlayerManager.GetPlayer(msgLogin.id);
                proto.LoginMsg.MsgKick msgKick = new proto.LoginMsg.MsgKick();
                msgKick.reson = 0;
                other.Send(msgKick);
                NetManager.Close(clientState);
            }

            PlayerData playerData = DataBaseManager.GetPlayerData(msgLogin.id);
            if (playerData == null)
            {
                msgLogin.result = 1;
                NetManager.Send(clientState, msgLogin);
                return;
            }

            Player player = new Player(clientState);
            player.id = msgLogin.id;
            player.data = playerData;
            PlayerManager.AddPlayer(msgLogin.id, player);
            clientState.player = player;
            msgLogin.result = 0;
            player.Send(msgLogin);
        }
    }
}