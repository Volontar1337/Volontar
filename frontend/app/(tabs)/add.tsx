import { ThemedText } from "@/components/ThemedText"
import React from "react"
import { SafeAreaView, StyleSheet, View } from "react-native"

const Separator = () => (
  <View style={{ height: 1, backgroundColor: "#E0E0E0", marginVertical: 16 }} />
)

export default function AddScreen() {
  return (
    <SafeAreaView style={styles.content}>
      <View
        style={{
          flex: 1,
          paddingHorizontal: 10,
          justifyContent: "center",
        }}
      >
        <ThemedText type="title">Add</ThemedText>
        <ThemedText>
          Here you can add your content, what you want help with etc etc..{" "}
        </ThemedText>
        <Separator />
        <ThemedText>
          This has a SafeAreaView and a custom view style that we wrap the text
          in to get that nice horizontal padding.
        </ThemedText>
      </View>
    </SafeAreaView>
  )
}

const styles = StyleSheet.create({
  content: {
    flex: 1,
  },
})
