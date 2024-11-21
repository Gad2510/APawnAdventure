
#Dependencies
import sys
import json
import numpy as np
import random
import math
from dataclasses import dataclass

@dataclass
class GraphValues:
    x0: any #Point where is going to start to calculate
    xI: int #Init of point of view 
    xE: int #End of point of view 
    isDesendent:bool

#Particle
class Particle():
	coord: np.ndarray
	value: float

	def __init__(self, _coord):
		self.coord=_coord
		
	def __str__(self):
		v=str(self.value) if hasattr(self,"value") else " Not assign"
		return "Coord: "+str(self.coord)+ " | Value:"+ v
	
	def returnJson(self):
		v=json.dumps({'value':float(self.value),'coord':self.coord.tolist()})
		return v
	
#Graph
class ParticleSwarnOptimisation():
	repeat:int =1
	step: float
	xL:list
	yL:list
	prop:GraphValues
	saturate:bool = True
	population_a:int=6
	#Particle def, Velocity, local info, f(x)
	population:list[Particle]
	xbest:Particle
	switchProb:float=0.5
	lamda:float=1.5
	sigma:float=0.5
	
	def __init__(self,xI,xE,**kwargs):
		 # Config with basic info for the graph
		self.ConfigGraph(xI,xE,**kwargs)

	def ConfigGraph(self, xI, xE, **kwargs):
		#Set properties
		self.population_a=kwargs["amount"] if "amount" in kwargs.keys() else self.population_a
		self.switchProb=kwargs["switch"] if "switch" in kwargs.keys() else self.switchProb
		
		#Used the base function and adds the dimetions to be used
		if "init" in kwargs.keys():
			self.dimention=len(kwargs["init"])
		else:
			self.dimention=kwargs["dimention"] if "dimention" in kwargs.keys() else self.dimention

		self.prop= GraphValues(
			None,
			# Minimun value for x
			xI,
			# Maximun value for x
			xE,
			# Direction of the iteration
			"desendent" not in kwargs.keys()
			)
		#Start position, can be set by user or random base in xI and xE 
		self.prop.x0=self.GetStartPos() if "init" not in kwargs.keys() else kwargs["init"]
		#Check if there is there a parameter for to restart
		self.repeat=1 if "repeat" not in kwargs.keys() else kwargs["repeat"]
		#Set step
		self.step=0.28 if "step" not in kwargs.keys() else kwargs["step"]
		
		#Create population
		self.xbest=Particle(np.zeros(self.dimention))
		p= self.CreatePopulation()
		self.AssignNewBest(p)
		return
	
	def LoadGraph(self):
		for i in range(self.repeat):
			self.ExecuteMethod()
			self.OnReapeat(i)
		self.OnFinish()

	def GetStartPos(self):
		return np.random.uniform(self.prop.xI,self.prop.xE,self.dimention)

	def Formula(self,x):
		fx=18.5
		fy=26
		s=100
		idx, val=x
		exp=val**3
		trian=0
		if(idx==0):
			trian=10*(math.cos(2*math.pi*val*fy)+math.sin(val*fx))
		else:
			trian=10*(math.sin(2*math.pi*val*fx)+math.cos(val*fy))
		return (exp*trian)/(2*(s**2))
	
	def MultiDimFormula(self,x):
		#Evaluate every element of the array into the formula
		indexed=enumerate(x)
		arr=np.fromiter(map(self.Formula,indexed),dtype=float)
		#Sum every element in the array
		return arr
	
	def CheckQuality(self,v,xT):#Check if the new iteration is better
		"""
    	Takes two values and evaluete the formula for both
		Then it returns if is best than the other
		param v: New Value
		param xT: Old Value 
    	"""
		#v: New value
		fv=sum(self.MultiDimFormula(v))
		#xT: old value
		fxT=sum(self.MultiDimFormula(xT))
		if(self.prop.isDesendent): #Decendent
			return (fv<fxT) #Check if the new iteration is better
		else: #Acendant 
			return (fv>fxT)
		
	def EvaluateGlobalBest(self):
		best=0
		#Interpretate values in array
		local_info=np.array([par.value for par in self.population])
		#Get index of the best solution in this generation
		if(self.prop.isDesendent):
			best=np.argmin(local_info)
		else:
			best=np.argmax(local_info)
		bestP=self.population[best]
		#Check if local best is better than the global
		b=self.CheckQuality(bestP.coord,self.xbest.coord)
		if(b):
			#Update global best
			self.AssignNewBest(bestP)
		return 0
	#Copy information from particle so is independent from the particle
	def AssignNewBest(self,_other:Particle):
		self.xbest.coord=_other.coord
		self.xbest.value=_other.value

	def ExecuteMethod(self):
		self.EvaluatePopulation()
		self.EvaluateValue()
		return 0 

	def EvaluateValue(self,*argv):
		self.EvaluateGlobalBest()
		return 0
	
	def OnReapeat(self, lap):
		return 0
	
	def OnFinish(self):
		return 0
	
	def CreatePopulation(self):
		self.population=[]
		xLocalBest:Particle=None
		for i in range(self.population_a):
			#Create a member of the poulation
			#Particle
			particule=Particle(self.GetStartPos())
			#Value
			particule.value=sum(self.MultiDimFormula(particule.coord))
			#Add to population
			self.population.append(particule)
			#Assing new local best
			if(xLocalBest==None or self.CheckQuality(particule.coord,xLocalBest.coord)):
				xLocalBest=particule
		return xLocalBest
	
	def EvaluatePopulation(self):
		for i in range(self.population_a):
			#Evaluate Particule
			self.EvaluateParticule(self.population,i)
		return 0
	
	def EvaluateParticule(self,_arr:list[Particle],_index):
		#Temp
		p:Particle=self.population[_index] 
		#Random value to cehck if is going globla or local
		r= random.random()
		if(r<self.switchProb):
			#global
			levy=self.LevyDistribution()
			p.coord=p.coord+(levy*(self.xbest.coord-p.coord))
		else:
			#local
			e=np.random.uniform(0,1,self.dimention)
			j:Particle=np.random.choice(self.population)
			p.coord=p.coord+(e*(j.coord-p.coord))
		
		if(self.saturate):
			p.coord=np.clip(p.coord,self.prop.xI,self.prop.xE)
			
		_arr[_index].coord=p.coord
		_arr[_index].value=sum(self.MultiDimFormula(p.coord))
		
	def LevyDistribution(self):
		l=(np.random.gamma( self.lamda)*np.sin(np.pi * self.lamda / 2)/np.pi)* (1/(self.sigma**(1+self.lamda)))
		return l

#gr=ParticleSwarnOptimisation(-5,5,repeat=100,amount=10,dimention=2,desendent=False)
#gr.LoadGraph()
#print(gr.xbest.returnJson())

if __name__ == "__main__":
	min = int(sys.argv[1])  # Min value to evaluate
	max = int(sys.argv[2])  # Max value to evaluate
	cicles=int(sys.argv[3])  # Assign number of generations
	population=int(sys.argv[4]) # Assign number of populations
	gr=ParticleSwarnOptimisation(min,max,repeat=cicles,amount=population,dimention=2,desendent=False)
	gr.LoadGraph()
	print(gr.xbest.returnJson())