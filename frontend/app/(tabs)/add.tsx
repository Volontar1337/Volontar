import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import React, { useState } from 'react';
import { Alert, KeyboardAvoidingView, Platform, ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View } from 'react-native';

import { Colors } from '@/constants/Colors';
import { useAuth } from '@/contexts/AuthContext';
import { hp, responsiveFontSize, responsiveSpacing, wp } from '@/utils/responsive';

export default function AddScreen() {
  const { createMission, user } = useAuth();
  const [title, setTitle] = useState('');
  const [category, setCategory] = useState('Hjälp i hemmet');
  const [description, setDescription] = useState('');
  const [location, setLocation] = useState('');
  const [date, setDate] = useState('');
  const [time, setTime] = useState('');
  const [maxParticipants, setMaxParticipants] = useState('');
  const [loading, setLoading] = useState(false);
  const [showCategoryDropdown, setShowCategoryDropdown] = useState(false);

  const categories = [
    'Hjälp i hemmet',
    'Transport',
    'Handla mat',
    'Städning',
    'Trädgårdsarbete',
    'IT-hjälp',
    'Språkstöd',
    'Barnpassning',
    'Äldrevård',
    'Annat'
  ];

  const handleSubmit = async () => {
    if (!title || !description || !location || !date || !time) {
      Alert.alert('Fel', 'Vänligen fyll i alla obligatoriska fält');
      return;
    }

    if (!user) {
      Alert.alert('Fel', 'Du måste vara inloggad för att skapa uppdrag');
      return;
    }

    setLoading(true);

    try {
      const success = await createMission({
        title,
        description,
        location,
        date,
        time,
        maxParticipants: maxParticipants ? parseInt(maxParticipants) : undefined,
      });

      if (success) {
        Alert.alert('Framgång!', 'Uppdraget har skapats', [
          {
            text: 'OK',
            onPress: () => {
              // Reset form
              setTitle('');
              setDescription('');
              setLocation('');
              setDate('');
              setTime('');
              setMaxParticipants('');
              setCategory('Hjälp i hemmet');
            }
          }
        ]);
      } else {
        Alert.alert('Fel', 'Kunde inte skapa uppdraget');
      }
    } catch (error) {
      Alert.alert('Fel', 'Något gick fel, försök igen');
    } finally {
      setLoading(false);
    }
  };

  return (
    <LinearGradient
      colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
      style={styles.container}
    >
      {/* Header with gradient */}
      <LinearGradient
        colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
        style={styles.header}
      >
        <Text style={styles.headerTitle}>Skapa nytt uppdrag</Text>
        <Text style={styles.headerSubtitle}>Beskriv vad du behöver hjälp med</Text>
      </LinearGradient>

      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        style={styles.keyboardView}
      >
        <ScrollView
          style={styles.content}
          contentContainerStyle={styles.scrollContent}
          showsVerticalScrollIndicator={false}
        >
          {/* Form */}
          <View style={styles.formContainer}>
            {/* Category Dropdown */}
            <View style={styles.inputGroup}>
              <Text style={styles.label}>Kategori *</Text>
              <TouchableOpacity
                style={styles.dropdownButton}
                onPress={() => setShowCategoryDropdown(!showCategoryDropdown)}
              >
                <Text style={styles.dropdownText}>{category}</Text>
                <Ionicons name="chevron-down" size={20} color={Colors.text.placeholder} />
              </TouchableOpacity>
              
              {showCategoryDropdown && (
                <View style={styles.dropdownMenu}>
                  {categories.map((cat) => (
                    <TouchableOpacity
                      key={cat}
                      style={styles.dropdownItem}
                      onPress={() => {
                        setCategory(cat);
                        setShowCategoryDropdown(false);
                      }}
                    >
                      <Text style={styles.dropdownItemText}>{cat}</Text>
                    </TouchableOpacity>
                  ))}
                </View>
              )}
            </View>

            {/* Title */}
            <View style={styles.inputGroup}>
              <Text style={styles.label}>Titel *</Text>
              <TextInput
                style={styles.input}
                placeholder="T.ex. Hjälp med inköp"
                placeholderTextColor={Colors.text.placeholder}
                value={title}
                onChangeText={setTitle}
                maxLength={100}
              />
            </View>

            {/* Description */}
            <View style={styles.inputGroup}>
              <Text style={styles.label}>Beskrivning *</Text>
              <TextInput
                style={[styles.input, styles.textArea]}
                placeholder="Beskriv vad du behöver hjälp med..."
                placeholderTextColor={Colors.text.placeholder}
                value={description}
                onChangeText={setDescription}
                multiline
                numberOfLines={4}
                textAlignVertical="top"
                maxLength={500}
              />
              <Text style={styles.characterCount}>
                {description.length}/500 tecken
              </Text>
            </View>

            {/* Location */}
            <View style={styles.inputGroup}>
              <Text style={styles.label}>Plats *</Text>
              <TextInput
                style={styles.input}
                placeholder="T.ex. Göteborg Centrum"
                placeholderTextColor={Colors.text.placeholder}
                value={location}
                onChangeText={setLocation}
                maxLength={100}
              />
            </View>

            {/* Date and Time */}
            <View style={styles.dateTimeContainer}>
              <View style={[styles.inputGroup, styles.halfWidth]}>
                <Text style={styles.label}>Datum *</Text>
                <TextInput
                  style={styles.input}
                  placeholder="YYYY-MM-DD"
                  placeholderTextColor={Colors.text.placeholder}
                  value={date}
                  onChangeText={setDate}
                />
              </View>

              <View style={[styles.inputGroup, styles.halfWidth]}>
                <Text style={styles.label}>Tid *</Text>
                <TextInput
                  style={styles.input}
                  placeholder="HH:MM"
                  placeholderTextColor={Colors.text.placeholder}
                  value={time}
                  onChangeText={setTime}
                />
              </View>
            </View>

            {/* Max Participants */}
            <View style={styles.inputGroup}>
              <Text style={styles.label}>Max antal deltagare (valfritt)</Text>
              <TextInput
                style={styles.input}
                placeholder="T.ex. 3"
                placeholderTextColor={Colors.text.placeholder}
                value={maxParticipants}
                onChangeText={setMaxParticipants}
                keyboardType="numeric"
                maxLength={2}
              />
            </View>

            {/* Submit Button */}
            <TouchableOpacity
              style={[styles.submitButton, loading && styles.submitButtonDisabled]}
              onPress={handleSubmit}
              disabled={loading}
            >
              <Text style={styles.submitButtonText}>
                {loading ? 'Skapar uppdrag...' : 'Skapa uppdrag'}
              </Text>
            </TouchableOpacity>

            {/* Info Text */}
            <View style={styles.infoContainer}>
              <Text style={styles.infoText}>
                * Obligatoriska fält
              </Text>
              <Text style={styles.infoText}>
                När du skapar ett uppdrag kommer det att synas för alla volontärer i området.
              </Text>
            </View>
          </View>

          {/* Bottom space for tab bar */}
          <View style={styles.bottomSpace} />
        </ScrollView>
      </KeyboardAvoidingView>
    </LinearGradient>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  header: {
    paddingTop: hp(6),
    paddingBottom: hp(3),
    paddingHorizontal: wp(5),
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: responsiveFontSize(24),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(8),
    textAlign: 'center',
  },
  headerSubtitle: {
    fontSize: responsiveFontSize(16),
    color: Colors.text.body,
    textAlign: 'center',
  },
  keyboardView: {
    flex: 1,
  },
  content: {
    flex: 1,
  },
  scrollContent: {
    paddingHorizontal: wp(5),
    paddingTop: hp(3),
  },
  formContainer: {
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    padding: responsiveSpacing(20),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  inputGroup: {
    marginBottom: responsiveSpacing(20),
  },
  label: {
    fontSize: responsiveFontSize(16),
    fontWeight: '600',
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(8),
  },
  input: {
    borderWidth: 1,
    borderColor: Colors.ui.lightGray,
    borderRadius: 20,
    paddingHorizontal: responsiveSpacing(16),
    paddingVertical: responsiveSpacing(14),
    fontSize: responsiveFontSize(16),
    backgroundColor: Colors.ui.white,
    color: Colors.text.body,
  },
  textArea: {
    height: 100,
    paddingTop: responsiveSpacing(14),
  },
  characterCount: {
    fontSize: responsiveFontSize(12),
    color: Colors.text.placeholder,
    textAlign: 'right',
    marginTop: responsiveSpacing(4),
  },
  dropdownButton: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: Colors.ui.lightGray,
    borderRadius: 20,
    paddingHorizontal: responsiveSpacing(16),
    paddingVertical: responsiveSpacing(14),
    backgroundColor: Colors.ui.white,
  },
  dropdownText: {
    fontSize: responsiveFontSize(16),
    color: Colors.text.body,
  },
  dropdownMenu: {
    position: 'absolute',
    top: '100%',
    left: 0,
    right: 0,
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    borderWidth: 1,
    borderColor: Colors.ui.lightGray,
    maxHeight: 200,
    zIndex: 1000,
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  dropdownItem: {
    paddingHorizontal: responsiveSpacing(16),
    paddingVertical: responsiveSpacing(12),
    borderBottomWidth: 1,
    borderBottomColor: Colors.ui.lightGray,
  },
  dropdownItemText: {
    fontSize: responsiveFontSize(16),
    color: Colors.text.body,
  },
  dateTimeContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    gap: responsiveSpacing(12),
  },
  halfWidth: {
    flex: 1,
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
  infoContainer: {
    marginTop: responsiveSpacing(20),
    paddingTop: responsiveSpacing(16),
    borderTopWidth: 1,
    borderTopColor: Colors.ui.lightGray,
  },
  infoText: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    lineHeight: responsiveFontSize(20),
    marginBottom: responsiveSpacing(8),
  },
  bottomSpace: {
    height: hp(10),
  },
});
