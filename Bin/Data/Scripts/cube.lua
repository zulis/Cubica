cube = {}
cube.__index = cube

function cube.create(target)
  local c = {}           -- our new object
  setmetatable(c, cube)  -- make cube handle lookup 
  c.cube = target        -- initialize our object
  
  c.cubeInitPos = Vector3d(0, 0, 0)
  c.cubeInitRot = Vector3d(0, 0, 0)
  c.gameWin = false
  c.gameLoose = false
  c.direction = "stop"
  c.prevDirection = c.direction
  c.step = 0
  c.rotateAround = Vector3d(0, 0, 0)
  c.speed = 3.75 --0,25|0,5|0,75|1|1,25|1,5|2|2,25|2,5|3|3,75|4,5|5|6|7,5|9|10|11,25|15|18|22,5|30|45|90|  
  c.impulseVec = Vector3d(0, 0, 0)
  
  c:init()
  return c
end

function cube:init()
  self.cube:SetMass(0)
  self.cubeInitPos = self.cube:GetPosition()
  self.cubeInitRot = self.cube:GetRotation()
  CubeStartSound:Play(1300)
  dbg("Scene Id", Menu:GetSceneId())
end

function cube:update()
  if self.direction=="stop"
  then
	local min = self.cube:GetMinBBox()
	local max = self.cube:GetMaxBBox()
	
	if not Menu:IsVisible()
	then	
	  if KeyDown("rightarrow") or JoyRightKeyDown() or GamepadRightKeyDown()
	  then
	    self.direction="right"
	    self.rotateAround = Vector3d(max.x, min.y, min.z)
	    playCubeRotateSound()
		UIInGame:DoStep()
	  elseif KeyDown("leftarrow") or JoyLeftKeyDown() or GamepadLeftKeyDown()
	  then
  	    self.direction="left"
	    self.rotateAround = Vector3d(min.x, min.y, min.z)
	    playCubeRotateSound()
		UIInGame:DoStep()
	  elseif KeyDown("uparrow") or JoyUpKeyDown() or GamepadDownKeyDown()
	  then
	    self.direction="up"
	    self.rotateAround = Vector3d(min.x, min.y, max.z)
	    playCubeRotateSound()
		UIInGame:DoStep()
	  elseif KeyDown("downarrow") or JoyDownKeyDown() or GamepadUpKeyDown()
	  then
	    self.direction="down"
	    self.rotateAround = Vector3d(min.x, min.y, min.z)
	    playCubeRotateSound()
		UIInGame:DoStep()
	  end
	end
	-- finish
	if self.cube:InteractsWith(Finish) and cube:stands() and not self.gameWin and not self.gameLoose
	then
	  self.gameWin = true
	  self.step = 0
	  CubeWinSound:Play()
	  
	  if Menu:IsLastScene()
	  then
	    Menu:Show("finish")
	  else
	    Menu:Show("win")
	  end
	end
	
	-- dead
	if not self.gameWin and not self.gameLoose 
    then
	  if hasCustParam(GetInteractingTriggers(self.cube), "type", "Dead") or
	    (hasCustParam(GetInteractingTriggers(self.cube), "type", "DeadWood") and
		cube:stands())
	  then
		GamepadVibrate()
	    self.gameLoose = true
		self.step = 0
	    self.cube:SetMass(10)
		FinishPrc:Stop()
		local impulseValue = 0.05
		if self.prevDirection == "left"
		then
		  self.impulseVec.x = -impulseValue
		elseif self.prevDirection == "right"
		then
		  self.impulseVec.x = impulseValue
		elseif self.prevDirection == "up"
		then
		  self.impulseVec.z = impulseValue
		elseif self.prevDirection == "down"
		then
		  self.impulseVec.z = -impulseValue
		end
		CubeDeadSound:Play()
	  end
	end
  end

  if not self.gameWin and not self.gameLoose
  then  
    cube:rotate()
  elseif self.gameWin
  then
    self.step = self.step + 1
	if self.step < 50
	then
      cube:moveDown()
	else
	  self.cube:SetMass(10)
	end
  elseif self.gameLoose
  then
    self.cube:SetImpulse(self.impulseVec)
	self.step = self.step + 1
	if self.step < 30
	then
	  camera:shake()
	elseif self.step == 40
	then
	  dropGroundObjects()
	elseif self.step == 60
	then
	  Menu:Show("dead")
	end  
  end	
end

function cube:rotate()
  dbg("direction", self.direction)
  
  if self.direction ~= "stop"
  then
    self.prevDirection = self.direction
  end
  
  dbg("prevDirection", self.prevDirection)
  
  if self.direction == "right"
  then
    self.step = self.step + self.speed
	self.cube:RotateZAroundPoint(self.rotateAround, -self.speed)
	if self.step == 90
	then
	  self.direction = "stop"
	  self.step = 0
	end
  elseif self.direction=="left"
  then
	self.step = self.step + self.speed
	self.cube:RotateZAroundPoint(self.rotateAround, self.speed)
	if self.step == 90
	then
	  self.direction = "stop"
	  self.step = 0
	end
  elseif self.direction=="down"
  then
	self.step = self.step + self.speed
	self.cube:RotateXAroundPoint(self.rotateAround, -self.speed)
	if self.step == 90
	then
	  self.direction = "stop"
	  self.step = 0
	end
  elseif self.direction == "up"
  then
	self.step = self.step + self.speed
	self.cube:RotateXAroundPoint(self.rotateAround, self.speed)
	if self.step == 90
	then
	  self.direction = "stop"
	  self.step = 0
	end
  elseif self.direction=="stop"
  then
    stands=cube:stands()
  end
  
  camera:update()
  
  dbg("stands", stands)
end

function cube:moveDown()
  self.cube:MoveY(-0.1)
end

function cube:restart()
  self.cube:SetMass(0)
  self.gameLoose = false
  self.cube:SetPosition(self.cubeInitPos.x, self.cubeInitPos.y, self.cubeInitPos.z)
  self.cube:SetRotation(self.cubeInitRot.x, self.cubeInitRot.y, self.cubeInitRot.z)
  CubeStartSound:Play()
end

function cube:stands()
  minBB = self.cube:GetMinBBox()
  maxBB = self.cube:GetMaxBBox()

  if(math.floor(math.abs(tonumber(maxBB.y)-tonumber(minBB.y))) < 3)
  then
    return false
  else
    return true
  end
end
