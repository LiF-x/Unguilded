/**
* <author>Warped Ibun</author>
* <email>lifxmod@gmail.com</email>
* <url>lifxmod.com</url>
* <credits></credits>
* <description>Disconnects Players During JH if they are not in a guild</description>
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/
if (!isObject(LiFx-Unguilded))
{
    new ScriptObject(LiFx-Unguilded)
    {
    };
}

package LiFx-Unguilded
{
    function LiFx-Unguilded::version()
    {
        return "v1.0.0";
    }

    function LiFx-Unguilded::setup()
    {
        LiFx::registerCallback($LiFx::hooks::onJHStartCallbacks, JHStatus, LiFx-Unguilded);
        LiFx::registerCallback($LiFx::hooks::onConnectCallbacks, checkGuildlessPlayers, LiFx-Unguilded);
    }

    function LiFx-Unguilded::JHStatus(%this)
    {
        if (IsJHActive())
        {
            // Judgement Hour is active Begin CheckGuildless players.
            %this.checkGuildlessPlayers();
        }

    }

    function LiFx-Unguilded::checkGuildlessPlayers(%this)
    {
        %query = "SELECT `ID`, `Name`, `LastName` FROM `character` WHERE `GuildID` IS NULL OR `GuildID` = ''";

        if (IsJHActive())
        {
            dbi.Select(LiFx-Unguilded, "kickguildlessPlayers", %query);
        }
        else
        {
            echo("Judgement Hour is not active. Skipping the check for unguilded players.");
        }

    }

    function LiFx-Unguilded::kickguildlessPlayers(%this, %resultSet)
    {
        if (%resultSet.ok())
        {
            while (%resultSet.nextRecord())
            {
                %client = LiFxUtility::getPlayer( %resultSet.getFieldValue("ID"));
                if (%client != 0)
                {
                    %client.scheduleDelete("You have been ejected from the server due to non guilded character", 100);
                    echo("LiFx|Unguilded Player found and removed!");
                }
            }
        }

        dbi.remove(%resultSet);
        %resultSet.delete();
    }
};

activatePackage(LiFx-Unguilded);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFx-Unguilded);
