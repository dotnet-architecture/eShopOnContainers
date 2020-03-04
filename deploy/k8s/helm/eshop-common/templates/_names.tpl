{{- define "imagePullSecret" }}
{{- printf "{\"auths\": {\"%s\": {\"auth\": \"%s\"}}}" .Values.inf.registry.server (printf "%s:%s" .Values.inf.registry.login .Values.inf.registry.pwd | b64enc) | b64enc }}
{{- end }}