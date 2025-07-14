import AsyncStorage from '@react-native-async-storage/async-storage';
import React, { createContext, ReactNode, useContext, useEffect, useState } from 'react';

import { CreateMissionData, Mission, RegisterData, User } from '@/types';

interface AuthContextType {
  // Auth state
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  
  // Auth actions
  login: (email: string, password: string) => Promise<boolean>;
  register: (userData: RegisterData) => Promise<boolean>;
  logout: () => Promise<void>;
  
  // Mission state (mock data)
  missions: Mission[];
  userMissions: Mission[];
  
  // Mission actions
  createMission: (missionData: CreateMissionData) => Promise<boolean>;
  joinMission: (missionId: string) => Promise<boolean>;
  leaveMission: (missionId: string) => Promise<boolean>;
  getMissionParticipants: (missionId: string) => User[];
}


const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Mock data
const mockUsers: User[] = [
  {
    id: '1',
    email: 'john@volunteer.com',
    firstName: 'John',
    lastName: 'Doe',
    role: 'volunteer',
  },
  {
    id: '2',
    email: 'org@redcross.com',
    firstName: 'Red Cross',
    lastName: 'Organization',
    role: 'organization',
  },
];

const mockMissions: Mission[] = [
  {
    id: '1',
    title: 'Hjälp äldre med inköp',
    description: 'Vi behöver volontärer som kan hjälpa äldre personer med deras veckohandling.',
    location: 'Stockholm Centrum',
    date: '2025-01-10',
    time: '10:00',
    createdBy: '2',
    createdByName: 'Red Cross Organization',
    participants: [],
    maxParticipants: 5,
  },
  {
    id: '2',
    title: 'Städa naturområde',
    description: 'Gemensam städinsats i Hagaparken för att hålla naturen ren.',
    location: 'Hagaparken, Stockholm',
    date: '2025-01-15',
    time: '09:00',
    createdBy: '2',
    createdByName: 'Red Cross Organization',
    participants: [],
    maxParticipants: 10,
  },
];

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [missions, setMissions] = useState<Mission[]>(mockMissions);

  // Load user from storage on app start
  useEffect(() => {
    loadUserFromStorage();
  }, []);

  const loadUserFromStorage = async () => {
    try {
      const userData = await AsyncStorage.getItem('user');
      if (userData) {
        setUser(JSON.parse(userData));
      }
    } catch (error) {
      console.error('Error loading user from storage:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const login = async (email: string, password: string): Promise<boolean> => {
    try {
      // Mock login - check against mock users
      const foundUser = mockUsers.find(u => u.email === email);
      if (foundUser && password === 'password') { // Simple mock password
        setUser(foundUser);
        await AsyncStorage.setItem('user', JSON.stringify(foundUser));
        return true;
      }
      return false;
    } catch (error) {
      console.error('Login error:', error);
      return false;
    }
  };

  const register = async (userData: RegisterData): Promise<boolean> => {
    try {
      // Mock registration
      const newUser: User = {
        id: Date.now().toString(),
        email: userData.email,
        firstName: userData.firstName,
        lastName: userData.lastName,
        role: userData.role,
      };
      
      mockUsers.push(newUser);
      setUser(newUser);
      await AsyncStorage.setItem('user', JSON.stringify(newUser));
      return true;
    } catch (error) {
      console.error('Registration error:', error);
      return false;
    }
  };

  const logout = async (): Promise<void> => {
    try {
      setUser(null);
      await AsyncStorage.removeItem('user');
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  const createMission = async (missionData: CreateMissionData): Promise<boolean> => {
    try {
      if (!user) return false;
      
      const newMission: Mission = {
        id: Date.now().toString(),
        ...missionData,
        createdBy: user.id,
        createdByName: `${user.firstName} ${user.lastName}`,
        participants: [],
      };
      
      setMissions(prev => [...prev, newMission]);
      return true;
    } catch (error) {
      console.error('Create mission error:', error);
      return false;
    }
  };

  const joinMission = async (missionId: string): Promise<boolean> => {
    try {
      if (!user) return false;
      
      setMissions(prev => prev.map(mission => {
        if (mission.id === missionId && !mission.participants.includes(user.id)) {
          return {
            ...mission,
            participants: [...mission.participants, user.id],
          };
        }
        return mission;
      }));
      return true;
    } catch (error) {
      console.error('Join mission error:', error);
      return false;
    }
  };

  const leaveMission = async (missionId: string): Promise<boolean> => {
    try {
      if (!user) return false;
      
      setMissions(prev => prev.map(mission => {
        if (mission.id === missionId) {
          return {
            ...mission,
            participants: mission.participants.filter(id => id !== user.id),
          };
        }
        return mission;
      }));
      return true;
    } catch (error) {
      console.error('Leave mission error:', error);
      return false;
    }
  };

  const getMissionParticipants = (missionId: string): User[] => {
    const mission = missions.find(m => m.id === missionId);
    if (!mission) return [];
    
    return mockUsers.filter(user => mission.participants.includes(user.id));
  };

  const userMissions = missions.filter(mission => 
    user && mission.participants.includes(user.id)
  );

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    register,
    logout,
    missions,
    userMissions,
    createMission,
    joinMission,
    leaveMission,
    getMissionParticipants,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
