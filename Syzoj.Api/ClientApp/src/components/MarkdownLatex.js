import React, { Component } from 'react';
import ReactMarkdown from 'react-markdown';
import MathJax from "@matejmazur/react-mathjax";
import RemarkMathPlugin from 'remark-math';
import { Table } from 'react-bootstrap';

export const MarkdownLatex = (props) => {
  const newProps = {
    ...props,
    plugins: [
      RemarkMathPlugin,
    ],
    renderers: {
      ...props.renderers,
      math: (props) =>
        <MathJax.Node>{props.value}</MathJax.Node>,
      inlineMath: (props) =>
        <MathJax.Node inline>{props.value}</MathJax.Node>,
      table: (props) =>
        <Table bordered striped responsive {...props}></Table>,
    }
  };
  return (
    <MathJax.Context input="tex">
      <ReactMarkdown {...newProps} />
    </MathJax.Context>
  );
};