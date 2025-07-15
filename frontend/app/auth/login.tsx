import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { router, useLocalSearchParams } from 'expo-router';
import React, { useState } from 'react';
import {
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  View
} from 'react-native';

import { VolontarLogo } from '@/components/VolontarLogo';
import { Colors } from '@/constants/Colors';
import { useAuth } from '@/contexts/AuthContext';
import { hp, responsiveFontSize, responsiveSpacing, wp } from '@/utils/responsive';

export default function LoginScreen() {
  const { role } = useLocalSearchParams<{ role?: string }>();
  const { login, register } = useAuth();
  
  const [isLogin, setIsLogin] = useState(true);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [loading, setLoading] = useState(false);
  const selectedRole = 'user';
  const [error, setError] = useState('');

  const handleSubmit = async () => {
    setError('');

    if (!email || !password) {
      setError('Vänligen fyll i alla fält');
      return;
    }

    if (!isLogin && (!firstName || !lastName)) {
      setError('Vänligen fyll i alla fält');
      return;
    }

    setLoading(true);
    console.log('🔐 handleSubmit start');

    try {
      let success = false;

      if (isLogin) {
        console.log('➡️ Försöker logga in med:', email);
        success = await login(email, password);
        console.log('✅ login() returned:', success);
        if (!success) {
          setError('Fel email eller lösenord');
        }
      } else {
        console.log('➡️ Försöker registrera ny användare');
        success = await register({
          email,
          password,
          firstName,
          lastName,
          role: selectedRole,
        });
        console.log('✅ register() returned:', success);
        if (!success) {
          setError('Registrering misslyckades');
        }
      }

      if (success) {
        console.log('➡️ Navigerar till /profile');
        setLoading(false);
        router.replace('/(tabs)/profile');
        return;
      }
    } catch (error) {
      console.error('❌ Något gick fel i handleSubmit:', error);
      setError('Något gick fel, försök igen');
    }

    setLoading(false); // Lägg till denna även om inget lyckas
  };


  return (
    <LinearGradient
      colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
      style={styles.container}
    >
      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        style={styles.keyboardView}
      >
        <ScrollView
          contentContainerStyle={styles.scrollContent}
          showsVerticalScrollIndicator={false}
        >
          {/* Logo */}
          <View style={styles.logoContainer}>
            <VolontarLogo size={80} showText={true} textColor={Colors.text.heading} />
          </View>

          {/* Form */}
          <View style={styles.formContainer}>
            {/* Error Message */}
            {error ? (
              <View style={styles.errorContainer}>
                <Text style={styles.errorText}>{error}</Text>
              </View>
            ) : null}

            <Text style={styles.formTitle}>
              {isLogin ? 'Logga in' : 'Skapa konto'}
            </Text>

            {!isLogin && (
              <>
                <View style={styles.inputContainer}>
                  <Ionicons name="person-outline" size={20} color={Colors.text.placeholder} style={styles.inputIcon} />
                  <TextInput
                    style={styles.input}
                    placeholder="Förnamn"
                    placeholderTextColor={Colors.text.placeholder}
                    value={firstName}
                    onChangeText={setFirstName}
                    autoCapitalize="words"
                  />
                </View>
                
                <View style={styles.inputContainer}>
                  <Ionicons name="person-outline" size={20} color={Colors.text.placeholder} style={styles.inputIcon} />
                  <TextInput
                    style={styles.input}
                    placeholder="Efternamn"
                    placeholderTextColor={Colors.text.placeholder}
                    value={lastName}
                    onChangeText={setLastName}
                    autoCapitalize="words"
                  />
                </View>
              </>
            )}

            <View style={styles.inputContainer}>
              <Ionicons name="mail-outline" size={20} color={Colors.text.placeholder} style={styles.inputIcon} />
              <TextInput
                style={styles.input}
                placeholder="E-post"
                placeholderTextColor={Colors.text.placeholder}
                value={email}
                onChangeText={setEmail}
                keyboardType="email-address"
                autoCapitalize="none"
                autoCorrect={false}
              />
            </View>

            <View style={styles.inputContainer}>
              <Ionicons name="lock-closed-outline" size={20} color={Colors.text.placeholder} style={styles.inputIcon} />
              <TextInput
                style={styles.input}
                placeholder="Lösenord"
                placeholderTextColor={Colors.text.placeholder}
                value={password}
                onChangeText={setPassword}
                secureTextEntry
                autoCapitalize="none"
              />
            </View>

            <TouchableOpacity
              style={[styles.submitButton, loading && styles.submitButtonDisabled]}
              onPress={handleSubmit}
              disabled={loading}
            >
              <Text style={styles.submitButtonText}>
                {loading ? 'Laddar...' : isLogin ? 'Logga in' : 'Skapa konto'}
              </Text>
            </TouchableOpacity>

            {/* Toggle between login/register */}
            <TouchableOpacity
              style={styles.toggleButton}
              onPress={() => setIsLogin(!isLogin)}
            >
              <Text style={styles.toggleButtonText}>
                {isLogin
                  ? 'Har du inget konto? Registrera dig'
                  : 'Har du redan ett konto? Logga in'}
              </Text>
            </TouchableOpacity>

            {/* Demo credentials */}
            {isLogin && (
              <View style={styles.demoContainer}>
                <Text style={styles.demoTitle}>Demo-konton:</Text>
                <Text style={styles.demoText}>Användare: neo@example.com</Text>
                <Text style={styles.demoText}>Lösenord: password</Text>
              </View>
            )}
          </View>

          {/* Back button */}
          <TouchableOpacity
            style={styles.backButton}
            onPress={() => router.back()}
          >
            <Ionicons name="arrow-back" size={20} color={Colors.text.body} />
            <Text style={styles.backButtonText}>Tillbaka</Text>
          </TouchableOpacity>
        </ScrollView>
      </KeyboardAvoidingView>
    </LinearGradient>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  keyboardView: {
    flex: 1,
  },
  scrollContent: {
    flexGrow: 1,
    paddingHorizontal: wp(8),
    paddingVertical: hp(3),
  },
  logoContainer: {
    alignItems: 'center',
    marginTop: hp(2),
    marginBottom: hp(3),
  },
  formContainer: {
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    paddingHorizontal: wp(6),
    paddingVertical: hp(3),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
    marginBottom: hp(2),
  },
  errorContainer: {
    backgroundColor: Colors.status.error,
    borderRadius: 20,
    paddingHorizontal: responsiveSpacing(16),
    paddingVertical: responsiveSpacing(12),
    marginBottom: responsiveSpacing(16),
  },
  errorText: {
    color: Colors.ui.white,
    fontSize: responsiveFontSize(14),
    fontWeight: '600',
    textAlign: 'center',
  },
  formTitle: {
    fontSize: responsiveFontSize(24),
    fontWeight: 'bold',
    color: Colors.text.heading,
    textAlign: 'center',
    marginBottom: hp(2),
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: Colors.ui.lightGray,
    borderRadius: 20,
    paddingHorizontal: responsiveSpacing(16),
    marginBottom: responsiveSpacing(16),
    backgroundColor: Colors.ui.white,
  },
  inputIcon: {
    marginRight: responsiveSpacing(12),
  },
  input: {
    flex: 1,
    paddingVertical: responsiveSpacing(14),
    fontSize: responsiveFontSize(16),
    color: Colors.text.body,
  },
  submitButton: {
    backgroundColor: Colors.primary.blue,
    borderRadius: 20,
    paddingVertical: responsiveSpacing(16),
    alignItems: 'center',
    marginTop: responsiveSpacing(8),
    minHeight: 48,
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 4,
  },
  submitButtonDisabled: {
    opacity: 0.6,
  },
  submitButtonText: {
    fontSize: responsiveFontSize(18),
    fontWeight: '600',
    color: Colors.ui.white,
  },
  toggleButton: {
    paddingVertical: responsiveSpacing(16),
    alignItems: 'center',
  },
  toggleButtonText: {
    fontSize: responsiveFontSize(14),
    color: Colors.primary.blue,
    textDecorationLine: 'underline',
  },
  demoContainer: {
    backgroundColor: Colors.gradient.start,
    borderRadius: 20,
    padding: responsiveSpacing(12),
    marginTop: responsiveSpacing(8),
  },
  demoTitle: {
    fontSize: responsiveFontSize(14),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(4),
  },
  demoText: {
    fontSize: responsiveFontSize(12),
    color: Colors.text.body,
    marginBottom: 2,
  },
  backButton: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'flex-start',
    paddingVertical: responsiveSpacing(12),
    paddingHorizontal: responsiveSpacing(16),
    gap: responsiveSpacing(8),
  },
  backButtonText: {
    fontSize: responsiveFontSize(16),
    color: Colors.text.body,
    fontWeight: '500',
  },
});
