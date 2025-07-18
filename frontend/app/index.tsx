import { useEffect } from 'react';
import { router } from 'expo-router';
import { ActivityIndicator, View } from 'react-native';
import { Colors } from '@/constants/Colors';

export default function IndexScreen() {
  useEffect(() => {
    // Direkt till login/welcome när appen startar
    router.replace('/auth/login');
  }, []);

  return (
    <View
      style={{
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: Colors.gradient.end,
      }}
    >
      <ActivityIndicator size="large" color={Colors.ui.white} />
    </View>
  );
}
