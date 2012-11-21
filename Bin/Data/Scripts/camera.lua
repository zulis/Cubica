camera = {}
camera.__index = camera

function camera.create(target)
  local c = {}             -- our new object
  setmetatable(c, camera)  -- make camera handle lookup 
  c.camera = target        -- initialize our object
  c.shaking=false
  c:init()
  return c
end

local camX
local camY
local camZ
local startCamZ
local sceneCenterX
local sceneCenterY
local sceneCenterZ
local boundsZ

function camera:init()
  camX = -3
  camY = 14
  
  local cubePos = Cube:GetPosition()
  local boundsX = Scene:GetXBounds()
  local boundsY = Scene:GetYBounds()
  boundsZ = Scene:GetZBounds()
  local sceneWidth = math.abs(boundsX.y - boundsX.x)
  local sceneDeph = math.abs(boundsZ.y - boundsZ.x)
  sceneCenterX = sceneWidth / 2
  sceneCenterY = boundsY.x
  sceneCenterZ = sceneDeph / 2
  
  camZ = -sceneWidth / 1.2 - sceneDeph / 5
  startCamZ = -sceneWidth * 1.6
  self.camera:SetPosition(sceneCenterX+camX, sceneCenterY+camY, boundsZ.x+startCamZ)
  self.camera:SetLookAt(sceneCenterX, sceneCenterY, sceneCenterZ)
  self.camera:SetViewFrustum(60)
end

function camera:update()
  local currentCamPos = self.camera:GetPosition()
 
  if not self.shaking
  then
    local cubePos = Cube:GetPosition()
    --cubePos.x = cubePos.x + camX
    --cubePos.z = cubePos.z + camZ
    local diffX = round(math.abs(currentCamPos.x - sceneCenterX), 2)
    local diffZ = round(math.abs(math.abs(currentCamPos.z) - math.abs(boundsZ.x+camZ)), 2)
    local stepX = 0.003 * diffX
    local stepZ = 0.005 * diffZ
	
    if currentCamPos.x < cubePos.x
    then
      currentCamPos.x = currentCamPos.x + stepX
    elseif currentCamPos.x > cubePos.x
    then
      currentCamPos.x = currentCamPos.x - stepX
    end
	
	if (boundsZ.x+camZ) > currentCamPos.z
	then
	  currentCamPos.z = currentCamPos.z + stepZ
	end
	
	--if (currentCamPos.z-sceneCenterZ) < camZ
	--then
	--  currentCamPos.z = currentCamPos.z + stepZ
	--end
  else
    if math.random() > 0.5
    then
      currentCamPos.x = currentCamPos.x + math.random()/2
    else
      currentCamPos.x = currentCamPos.x - math.random()/2
    end
  	
    if math.random() > 0.5
    then
      currentCamPos.y = currentCamPos.y + math.random()/2
    else
      currentCamPos.y = currentCamPos.y - math.random()/2
    end
	self.shaking=false
  end
  
  self.camera:SetPosition(currentCamPos.x, currentCamPos.y, currentCamPos.z)
  self.camera:SetLookAt(sceneCenterX, sceneCenterY, sceneCenterZ)
end

function camera:shake()
  self.shaking=true
  camera:update()
end
