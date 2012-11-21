dofile( "data/scripts/sounds.lua" )
dofile( "data/scripts/camera.lua" )
dofile( "data/scripts/cube.lua" )

local music = {"data/sounds/music/music1.ogg", "data/sounds/music/music3.ogg", "data/sounds/music/music3.ogg",
               "data/sounds/music/music4.ogg", "data/sounds/music/music5.ogg"}

function main()
  if startupScene ~= nil
  then
    Game:LoadLevel(startupScene)
    Game:SetTriggersVisible(false)
  end
  
  if Menu:IsTutorial() == true
  then
    Menu:Show("tutorial")
  end
end
main()

function loop()
  local gameState = enumtostring(Menu:GetState())
  dbg("State", gameState)
  
  if Sound:IsMusicPlaying() == false
  then
    local idx = math.random(1, table.getn(music))
    Sound:PlayMusic(music[idx])
  end
  
  if KeyDown("escape") or JoyButtonDown(1)
  then
    if not Menu:IsVisible()
	then
	  if gameState == "Playing" or gameState == "Tutorial"
	  then
	    Menu:Show("pause")
	  end
	end
  end
end

-----------------------------------------------
-- globals ------------------------------------
-----------------------------------------------
local ground_objects = { }
function registerGroundObject(obj)
  obj:SetMass(0)
  table.insert(ground_objects, obj)
end

function dropGroundObjects()
  for k,v in pairs(ground_objects) do
	v:SetMass(10)
	local impulseVec = Vector3d(math.random(-5, 5), math.random(15), math.random(-5, 5))
	v:SetImpulse(impulseVec)
  end
end

-----------------------------------------------
-- helpers ------------------------------------
-----------------------------------------------
function round(num, idp)
  local mult = 10^(idp or 0)
  return math.floor(num * mult + 0.5) / mult
end

function hasCustParam(t, k, v)
  for kk,vv in pairs(t) do
    if vv:GetCustParam(k) == v
	then
	  return true
    end
  end
  return false
end

function split(str, pat)
   local t = {}
   local fpat = "(.-)" .. pat
   local last_end = 1
   local s, e, cap = str:find(fpat, 1)
   while s do
      if s ~= 1 or cap ~= "" then
	 table.insert(t,cap)
      end
      last_end = e+1
      s, e, cap = str:find(fpat, last_end)
   end
   if last_end <= #str then
      cap = str:sub(last_end)
      table.insert(t, cap)
   end
   return t
end

function enumtostring(v)
  return split(tostring(v), ":")[1]
end
