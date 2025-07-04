import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import React, { useState } from 'react';
import { ScrollView, StyleSheet, Text, TextInput, TouchableOpacity, View } from 'react-native';

import { Colors } from '@/constants/Colors';
import { useAuth } from '@/contexts/AuthContext';
import { hp, responsiveFontSize, responsiveSpacing, wp } from '@/utils/responsive';

export default function SearchScreen() {
  const { missions, joinMission, leaveMission, user } = useAuth();
  const [searchText, setSearchText] = useState('');
  const [showOrganizations, setShowOrganizations] = useState(true);
  const [showPrivatPersons, setShowPrivatPersons] = useState(true);

  // Filter missions based on search and filters
  const filteredMissions = missions.filter(mission => {
    const matchesSearch = mission.title.toLowerCase().includes(searchText.toLowerCase()) ||
                         mission.description.toLowerCase().includes(searchText.toLowerCase()) ||
                         mission.location.toLowerCase().includes(searchText.toLowerCase());
    
    const matchesFilter = showOrganizations;
    
    return matchesSearch && matchesFilter;
  });

  const handleJoinMission = async (missionId: string) => {
    if (!user) return;
    
    const mission = missions.find(m => m.id === missionId);
    if (!mission) return;
    
    const isJoined = mission.participants.includes(user.id);
    
    if (isJoined) {
      await leaveMission(missionId);
    } else {
      await joinMission(missionId);
    }
  };

  const renderMissionCard = (mission: any) => {
    const isJoined = user ? mission.participants.includes(user.id) : false;
    const spotsLeft = mission.maxParticipants ? mission.maxParticipants - mission.participants.length : null;

    return (
      <View key={mission.id} style={styles.missionCard}>
        {/* Mission Image */}
        <View style={styles.missionImageContainer}>
          <View style={styles.missionImage}>
            <Ionicons name="image-outline" size={32} color={Colors.ui.gray} />
          </View>
        </View>

        {/* Mission Content */}
        <View style={styles.missionContent}>
          <View style={styles.missionHeader}>
            <Text style={styles.missionTitle}>{mission.title}</Text>
            <View style={styles.organizationBadge}>
              <Text style={styles.organizationText}>Organisation</Text>
            </View>
          </View>

          <Text style={styles.missionDescription} numberOfLines={2}>
            {mission.description}
          </Text>

          <View style={styles.missionDetails}>
            <Text style={styles.missionLocation}>📍 {mission.location}</Text>
            <Text style={styles.missionDate}>📅 {mission.date} kl {mission.time}</Text>
            <Text style={styles.missionCreator}>👤 {mission.createdByName}</Text>
            {spotsLeft !== null && (
              <Text style={styles.spotsLeft}>
                {spotsLeft > 0 ? `${spotsLeft} platser kvar` : 'Fullt'}
              </Text>
            )}
          </View>
        </View>

        {/* Join Button */}
        <TouchableOpacity
          style={[
            styles.joinButton,
            isJoined ? styles.leaveButton : styles.joinButtonActive,
            spotsLeft === 0 && !isJoined ? styles.joinButtonDisabled : null
          ]}
          onPress={() => handleJoinMission(mission.id)}
          disabled={spotsLeft === 0 && !isJoined}
        >
          <Text style={[
            styles.joinButtonText,
            isJoined ? styles.leaveButtonText : styles.joinButtonActiveText
          ]}>
            {isJoined ? 'Lämna' : spotsLeft === 0 ? 'Fullt' : 'Gå med'}
          </Text>
        </TouchableOpacity>
      </View>
    );
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
        <Text style={styles.headerTitle}>Sök uppdrag</Text>
        
        {/* Search Bar */}
        <View style={styles.searchContainer}>
          <TextInput
            style={styles.searchInput}
            placeholder="Sök uppdrag eller ort/område..."
            placeholderTextColor={Colors.text.placeholder}
            value={searchText}
            onChangeText={setSearchText}
          />
          <Ionicons name="search-outline" size={20} color={Colors.text.placeholder} style={styles.searchIcon} />
        </View>

        {/* Filter Buttons */}
        <View style={styles.filterContainer}>
          <Text style={styles.filterLabel}>Filter</Text>
          <View style={styles.filterButtons}>
            <TouchableOpacity
              style={[
                styles.filterButton,
                showOrganizations ? styles.filterButtonActive : styles.filterButtonInactive
              ]}
              onPress={() => setShowOrganizations(!showOrganizations)}
            >
              <Text style={[
                styles.filterButtonText,
                showOrganizations ? styles.filterButtonTextActive : styles.filterButtonTextInactive
              ]}>
                Organisationer
              </Text>
            </TouchableOpacity>
            
            <TouchableOpacity
              style={[
                styles.filterButton,
                showPrivatPersons ? styles.filterButtonActive : styles.filterButtonInactive
              ]}
              onPress={() => setShowPrivatPersons(!showPrivatPersons)}
            >
              <Text style={[
                styles.filterButtonText,
                showPrivatPersons ? styles.filterButtonTextActive : styles.filterButtonTextInactive
              ]}>
                Privatpersoner
              </Text>
            </TouchableOpacity>
          </View>
        </View>
      </LinearGradient>

      {/* Map Placeholder */}
      <View style={styles.mapContainer}>
        <View style={styles.mapPlaceholder}>
          <Text style={styles.mapText}>🗺️ Göteborg</Text>
          <Text style={styles.mapSubtext}>Google Maps</Text>
          
          {/* Map Pins */}
          <View style={styles.mapPins}>
            <View style={[styles.mapPin, { top: '30%', left: '40%' }]}>
              <Ionicons name="location" size={24} color={Colors.primary.blue} />
            </View>
            <View style={[styles.mapPin, { top: '60%', left: '60%' }]}>
              <Ionicons name="location" size={24} color={Colors.primary.orange} />
            </View>
            <View style={[styles.mapPin, { top: '45%', left: '25%' }]}>
              <Ionicons name="location" size={24} color={Colors.primary.blue} />
            </View>
          </View>
        </View>
      </View>

      {/* Mission List */}
      <ScrollView style={styles.missionList} showsVerticalScrollIndicator={false}>
        <Text style={styles.resultsCount}>
          {filteredMissions.length} uppdrag hittades
        </Text>
        
        {filteredMissions.map(renderMissionCard)}
        
        {filteredMissions.length === 0 && (
          <View style={styles.emptyState}>
            <Ionicons name="search-outline" size={48} color={Colors.ui.gray} />
            <Text style={styles.emptyStateText}>Inga uppdrag hittades</Text>
            <Text style={styles.emptyStateSubtext}>
              Prova att ändra dina sökkriterier eller filter
            </Text>
          </View>
        )}
        
        {/* Bottom space for tab bar */}
        <View style={styles.bottomSpace} />
      </ScrollView>
    </LinearGradient>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  header: {
    paddingTop: hp(6),
    paddingBottom: hp(2),
    paddingHorizontal: wp(5),
  },
  headerTitle: {
    fontSize: responsiveFontSize(24),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: hp(2),
    textAlign: 'center',
  },
  searchContainer: {
    position: 'relative',
    marginBottom: hp(2),
  },
  searchInput: {
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    paddingHorizontal: responsiveSpacing(16),
    paddingVertical: responsiveSpacing(12),
    paddingRight: responsiveSpacing(48),
    fontSize: responsiveFontSize(16),
    borderWidth: 1,
    borderColor: Colors.ui.lightGray,
  },
  searchIcon: {
    position: 'absolute',
    right: responsiveSpacing(16),
    top: '50%',
    transform: [{ translateY: -10 }],
  },
  filterContainer: {
    marginBottom: hp(1),
  },
  filterLabel: {
    fontSize: responsiveFontSize(16),
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(8),
    fontWeight: '600',
  },
  filterButtons: {
    flexDirection: 'row',
    gap: responsiveSpacing(12),
  },
  filterButton: {
    paddingHorizontal: responsiveSpacing(16),
    paddingVertical: responsiveSpacing(8),
    borderRadius: 20,
    borderWidth: 1,
  },
  filterButtonActive: {
    backgroundColor: Colors.ui.white,
    borderColor: Colors.primary.blue,
  },
  filterButtonInactive: {
    backgroundColor: Colors.ui.white,
    borderColor: Colors.ui.lightGray,
  },
  filterButtonText: {
    fontSize: responsiveFontSize(14),
    fontWeight: '500',
  },
  filterButtonTextActive: {
    color: Colors.primary.blue,
  },
  filterButtonTextInactive: {
    color: Colors.text.body,
  },
  mapContainer: {
    height: hp(25),
  },
  mapPlaceholder: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    position: 'relative',
    backgroundColor: '#E8F4FD',
    margin: wp(5),
    borderRadius: 12,
  },
  mapText: {
    fontSize: responsiveFontSize(20),
    fontWeight: 'bold',
    color: Colors.text.heading,
  },
  mapSubtext: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    marginTop: responsiveSpacing(4),
  },
  mapPins: {
    position: 'absolute',
    width: '100%',
    height: '100%',
  },
  mapPin: {
    position: 'absolute',
  },
  missionList: {
    flex: 1,
    paddingHorizontal: wp(5),
  },
  resultsCount: {
    fontSize: responsiveFontSize(16),
    fontWeight: '600',
    color: Colors.text.heading,
    marginVertical: hp(2),
  },
  missionCard: {
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    marginBottom: responsiveSpacing(16),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
    flexDirection: 'row',
    overflow: 'hidden',
  },
  missionImageContainer: {
    width: 80,
    height: 120,
  },
  missionImage: {
    flex: 1,
    backgroundColor: Colors.ui.lightGray,
    alignItems: 'center',
    justifyContent: 'center',
  },
  missionContent: {
    flex: 1,
    padding: responsiveSpacing(16),
  },
  missionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: responsiveSpacing(8),
  },
  missionTitle: {
    fontSize: responsiveFontSize(16),
    fontWeight: 'bold',
    color: Colors.text.heading,
    flex: 1,
    marginRight: responsiveSpacing(8),
  },
  organizationBadge: {
    backgroundColor: Colors.primary.orange,
    borderRadius: 12,
    paddingHorizontal: responsiveSpacing(8),
    paddingVertical: responsiveSpacing(4),
  },
  organizationText: {
    fontSize: responsiveFontSize(10),
    color: Colors.ui.white,
    fontWeight: '600',
  },
  missionDescription: {
    fontSize: responsiveFontSize(12),
    color: Colors.text.body,
    lineHeight: responsiveFontSize(16),
    marginBottom: responsiveSpacing(8),
  },
  missionDetails: {
    marginBottom: responsiveSpacing(8),
  },
  missionLocation: {
    fontSize: responsiveFontSize(12),
    color: Colors.text.body,
    marginBottom: responsiveSpacing(2),
  },
  missionDate: {
    fontSize: responsiveFontSize(12),
    color: Colors.text.body,
    marginBottom: responsiveSpacing(2),
  },
  missionCreator: {
    fontSize: responsiveFontSize(12),
    color: Colors.text.body,
    marginBottom: responsiveSpacing(2),
  },
  spotsLeft: {
    fontSize: responsiveFontSize(12),
    color: Colors.primary.orange,
    fontWeight: '600',
  },
  joinButton: {
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: responsiveSpacing(16),
    minWidth: 80,
  },
  joinButtonActive: {
    backgroundColor: Colors.primary.orange,
  },
  leaveButton: {
    backgroundColor: Colors.status.error,
  },
  joinButtonDisabled: {
    backgroundColor: Colors.ui.lightGray,
  },
  joinButtonText: {
    fontSize: responsiveFontSize(12),
    fontWeight: '600',
  },
  joinButtonActiveText: {
    color: Colors.ui.white,
  },
  leaveButtonText: {
    color: Colors.ui.white,
  },
  emptyState: {
    alignItems: 'center',
    paddingVertical: hp(8),
  },
  emptyStateText: {
    fontSize: responsiveFontSize(18),
    fontWeight: 'bold',
    color: Colors.text.body,
    textAlign: 'center',
    marginTop: responsiveSpacing(16),
  },
  emptyStateSubtext: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    textAlign: 'center',
    marginTop: responsiveSpacing(8),
    paddingHorizontal: wp(5),
  },
  bottomSpace: {
    height: hp(10),
  },
});
