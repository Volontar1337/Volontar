import { router } from 'expo-router';
import React, { useEffect } from 'react';
import { ActivityIndicator, View } from 'react-native';

import { Colors } from '@/constants/Colors';
import { useAuth } from '@/contexts/AuthContext';

export default function IndexScreen() {
  const { isAuthenticated, isLoading } = useAuth();

  useEffect(() => {
    if (!isLoading) {
      if (isAuthenticated) {
        // ✅ Inloggad → till profil
        router.replace('/(tabs)/profile');
      } else {
        // 🚪 Inte inloggad → till login eller onboarding
        router.replace('/auth/login');
      }
    }
  }, [isAuthenticated, isLoading]);

  // Show loading spinner while checking auth status
  return (
    <View style={{
      flex: 1,
      justifyContent: 'center',
      alignItems: 'center',
      backgroundColor: Colors.gradient.end,
    }}>
      <ActivityIndicator size="large" color={Colors.ui.white} />
    </View>
  );
}
