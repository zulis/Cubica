local cube_sounds = { on_stone={}, on_metal={}, on_wood={} }

function registerSound(sound)
  local type
  type = sound:GetCustParam("type")
  if type=="stone"
  then
    table.insert(cube_sounds.on_stone, sound)
  elseif type=="metal"
  then
    table.insert(cube_sounds.on_metal, sound)
  elseif type=="wood"
  then
    table.insert(cube_sounds.on_wood, sound)
  end
end

function playCubeRotateSound()
  local interact = GetInteractingTriggers(Cube)
  for k,v in pairs(interact) do
    local type = v:GetCustParam("type")
    local idx
	
    if type=="stone"
	then
  	  idx = math.random(1, #cube_sounds.on_stone)
	  cube_sounds.on_stone[idx]:Play()
	elseif  type=="metal"
	then
	  idx = math.random(1, #cube_sounds.on_metal)
	  cube_sounds.on_metal[idx]:Play()
	elseif  type=="wood"
	then
	  idx = math.random(1, #cube_sounds.on_wood)
	  cube_sounds.on_wood[idx]:Play()
	end
  end  
end